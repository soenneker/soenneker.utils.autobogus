using System;
using System.Collections.Generic;
using Bogus;
using Soenneker.Reflection.Cache.Types;
using Soenneker.Utils.AutoBogus.Config;
using Soenneker.Utils.AutoBogus.Context;
using Soenneker.Utils.AutoBogus.Generators;
using Soenneker.Utils.AutoBogus.Generators.Abstract;
using Soenneker.Utils.AutoBogus.Services;

namespace Soenneker.Utils.AutoBogus;

/// <summary>
/// A class used to invoke generation requests of type <typeparamref name="TType"/>.
/// </summary>
/// <typeparam name="TType">The type of instance to generate.</typeparam>
public class AutoFaker<TType> : Faker<TType> where TType : class
{
    public AutoFakerConfig Config { get; set; }

    /// <summary>
    /// The <see cref="AutoFakerBinder"/> instance to use for the generation request.
    /// </summary>
    public AutoFakerBinder? Binder { get; set; }

    private bool _createInitialized;

    private bool _finishInitialized;

    private readonly Func<Faker, TType> _defaultCreateAction;

    private CacheService? _cacheService;

    public AutoFaker(AutoFakerConfig? autoFakerConfig = null)
    {
        if (autoFakerConfig == null)
            Config = new AutoFakerConfig();
        else
            Config = autoFakerConfig;

        Locale = Config.Locale;
        binder = new Binder();

        // Ensure the default create action is cleared
        // This is so we can check whether it has been set externally
        _defaultCreateAction = CreateActions[currentRuleSet];
        CreateActions[currentRuleSet] = null;
    }

    public void Initialize()
    {
        _cacheService ??= new CacheService(Config.ReflectionCacheOptions);
        Binder ??= new AutoFakerBinder();
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="ruleSets">An optional list of delimited rule sets to use for the generate request.</param>
    /// <returns>The generated instance of type <typeparamref name="TType"/>.</returns>
    public override TType Generate(string? ruleSets = null)
    {
        Initialize();

        AutoFakerContext context = CreateContext(ruleSets);

        PrepareCreate(context);
        PrepareFinish(context);

        return base.Generate(ruleSets);
    }

    /// <summary>
    /// Generates a collection of instances of type <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="count">The number of instances to generate.</param>
    /// <param name="ruleSets">An optional list of delimited rule sets to use for the generate request.</param>
    /// <returns>The collection of generated instances of type <typeparamref name="TType"/>.</returns>
    public override List<TType> Generate(int count, string? ruleSets = null)
    {
        Initialize();

        AutoFakerContext context = CreateContext(ruleSets);

        PrepareCreate(context);
        PrepareFinish(context);

        return base.Generate(count, ruleSets);
    }

    /// <summary>
    /// Populates the provided instance with auto generated values.
    /// </summary>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="ruleSets">An optional list of delimited rule sets to use for the populate request.</param>
    public override void Populate(TType instance, string? ruleSets = null)
    {
        AutoFakerContext context = CreateContext(ruleSets);
        PrepareFinish(context);

        base.Populate(instance, ruleSets);
    }

    private AutoFakerContext CreateContext(string? ruleSets)
    {
        return new AutoFakerContext(Config!, Binder, FakerHub, _cacheService)
        {
            RuleSets = ParseRuleSets(ruleSets)
        };
    }

    /// <summary>
    /// Parse and clean the rule set list
    /// If the rule set list is empty it defaults to a list containing only 'default'
    /// By this point the currentRuleSet should be 'default'
    /// </summary>
    /// <param name="ruleSets"></param>
    /// <returns></returns>
    private HashSet<string> ParseRuleSets(string? ruleSets)
    {
        if (string.IsNullOrWhiteSpace(ruleSets))
        {
            ruleSets = null;
        }

        var validRuleSets = new HashSet<string>();

        string[] ruleSetArray = ruleSets?.Split(',') ?? [currentRuleSet];

        for (var i = 0; i < ruleSetArray.Length; i++)
        {
            string trimmedRuleSet = ruleSetArray[i].Trim();

            if (!string.IsNullOrWhiteSpace(trimmedRuleSet))
            {
                validRuleSets.Add(trimmedRuleSet);
            }
        }

        return validRuleSets;
    }

    private void PrepareCreate(AutoFakerContext context)
    {
        // Check a create handler hasn't previously been set or configured externally
        if (_createInitialized || CreateActions[currentRuleSet] != null)
            return;

        CreateActions[currentRuleSet] = faker =>
        {
            // Only auto create if the 'default' rule set is defined for generation
            // This is because any specific rule sets are expected to handle the full creation
            if (context.RuleSets != null && context.RuleSets.Contains(currentRuleSet))
            {
                Type type = typeof(TType);

                CachedType cachedType = context.CacheService.Cache.GetCachedType(type);

                // Set the current type being generated
                context.ParentType = null;
                context.CachedType = cachedType;
                context.GenerateName = null;

                // Get the members that should not be set during construction
                HashSet<string> memberNames = GetRuleSetsMemberNames(context);

                foreach (string? memberName in TypeProperties.Keys)
                {
                    if (!memberNames.Contains(memberName))
                        continue;

                    var path = $"{type.FullName}.{memberName}";

                    if (context.Config.SkipPaths == null)
                    {
                        context.Config.SkipPaths = [path];
                        continue;
                    }

                    bool existing = context.Config.SkipPaths.Contains(path);

                    if (!existing)
                    {
                        context.Config.SkipPaths.Add(path);
                    }
                }

                // Create a blank instance. It will be populated in the FinalizeAction registered
                // by PrepareFinish (context.Binder.PopulateInstance<TType>).

                return context.Binder.CreateInstanceWithRecursionGuard<TType>(context, cachedType)!;
            }

            return _defaultCreateAction.Invoke(faker);
        };

        _createInitialized = true;
    }

    private void PrepareFinish(AutoFakerContext context)
    {
        if (_finishInitialized)
            return;

        FinalizeActions.TryGetValue(currentRuleSet, out FinalizeAction<TType>? finishWith);

        FinishWith((faker, instance) =>
        {
            if (instance == null)
                return;

            CachedType cachedType = context.CacheService.Cache.GetCachedType(instance.GetType());

            if (cachedType.IsExpandoObject)
            {
                context.ParentType = null;
                context.CachedType = cachedType;

                context.GenerateName = null;
                context.Instance = instance;

                IAutoFakerGenerator generator = AutoFakerGeneratorFactory.GetGenerator(context);
                generator.Generate(context);

                context.Instance = null;
                return;
            }

            List<AutoMember>? autoMembers = context.Binder.GetMembersToPopulate(cachedType, _cacheService, Config);

            if (autoMembers != null)
            {
                var finalAutoMembers = new List<AutoMember>();

                HashSet<string> memberNames = GetRuleSetsMemberNames(context);

                for (var i = 0; i < autoMembers.Count; i++)
                {
                    AutoMember autoMember = autoMembers[i];
                    if (!memberNames.Contains(autoMember.Name))
                    {
                        finalAutoMembers.Add(autoMember);
                    }
                }

                context.RecursiveConstructorStack.Clear();
                AutoFakerBinder.PopulateMembers(instance, context, cachedType, finalAutoMembers);
            }

            // Ensure the default finish with is invoke
            finishWith?.Action(faker, instance);
        });

        _finishInitialized = true;
    }

    private HashSet<string> GetRuleSetsMemberNames(AutoFakerContext context)
    {
        if (context.RuleSets == null)
            return [];

        var members = new HashSet<string>(context.RuleSets.Count);

        foreach (string ruleSetName in context.RuleSets)
        {
            if (!Actions.TryGetValue(ruleSetName, out Dictionary<string, PopulateAction<TType>>? ruleSet))
            {
                continue;
            }

            foreach (KeyValuePair<string, PopulateAction<TType>> keyValuePair in ruleSet)
            {
                members.Add(keyValuePair.Key);
            }
        }

        return members;
    }
}