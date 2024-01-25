using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bogus;
using Soenneker.Utils.AutoBogus.Abstract;
using Soenneker.Utils.AutoBogus.Extensions;

namespace Soenneker.Utils.AutoBogus;

/// <summary>
/// A class used to invoke generation requests of type <typeparamref name="TType"/>.
/// </summary>
/// <typeparam name="TType">The type of instance to generate.</typeparam>
public class AutoFaker<TType>
    : Faker<TType>
    where TType : class
{
    private AutoConfig _config;

    /// <summary>
    /// Instantiates an instance of the <see cref="AutoFaker{TType}"/> class.
    /// </summary>
    public AutoFaker()
        : this(null, null)
    { }

    /// <summary>
    /// Instantiates an instance of the <see cref="AutoFaker{TType}"/> class.
    /// </summary>
    /// <param name="locale">The locale to use for value generation.</param>
    public AutoFaker(string locale)
        : this(locale, null)
    { }

    /// <summary>
    /// Instantiates an instance of the <see cref="AutoFaker{TType}"/> class.
    /// </summary>
    /// <param name="binder">The <see cref="IAutoBinder"/> instance to use for the generation request.</param>
    public AutoFaker(IAutoBinder binder)
        : this(null, binder)
    { }

    /// <summary>
    /// Instantiates an instance of the <see cref="AutoFaker{TType}"/> class.
    /// </summary>
    /// <param name="locale">The locale to use for value generation.</param>
    /// <param name="binder">The <see cref="IAutoBinder"/> instance to use for the generation request.</param>
    public AutoFaker(string locale = null, IAutoBinder binder = null)
        : base(locale ?? AutoConfig.DefaultLocale, binder)
    {
        Binder = binder;

        // Ensure the default create action is cleared
        // This is so we can check whether it has been set externally
        DefaultCreateAction = CreateActions[currentRuleSet];
        CreateActions[currentRuleSet] = null;
    }

    /// <summary>
    /// The <see cref="IAutoBinder"/> instance to use for the generation request.
    /// </summary>
    public IAutoBinder Binder { get; set; }

    internal AutoConfig Config
    {
        set
        {
            _config = value;

            Locale = _config.Locale;
            Binder = _config.Binder;

            // Also pass the binder set up to the underlying Faker
            binder = _config.Binder;

            // Apply a configured faker if set
            if (_config.FakerHub != null)
            {
                FakerHub = _config.FakerHub;
            }
        }
    }

    private bool CreateInitialized { get; set; }
    private bool FinishInitialized { get; set; }
    private Func<Faker, TType> DefaultCreateAction { get; set; }

    /// <summary>
    /// Configures the current faker instance.
    /// </summary>
    /// <param name="configure">A handler to build the faker configuration.</param>
    /// <returns>The current faker instance.</returns>
    public AutoFaker<TType> Configure(Action<IAutoGenerateConfigBuilder> configure)
    {
        var config = new AutoConfig(AutoFaker.DefaultConfig);
        var builder = new AutoConfigBuilder(config);

        configure?.Invoke(builder);

        Config = config;

        return this;
    }

    /// <summary>
    /// Generates an instance of type <typeparamref name="TType"/>.
    /// </summary>
    /// <param name="ruleSets">An optional list of delimited rule sets to use for the generate request.</param>
    /// <returns>The generated instance of type <typeparamref name="TType"/>.</returns>
    public override TType Generate(string ruleSets = null)
    {
        AutoGenerateContext context = CreateContext(ruleSets);

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
    public override List<TType> Generate(int count, string ruleSets = null)
    {
        AutoGenerateContext context = CreateContext(ruleSets);

        PrepareCreate(context);
        PrepareFinish(context);

        return base.Generate(count, ruleSets);
    }

    /// <summary>
    /// Populates the provided instance with auto generated values.
    /// </summary>
    /// <param name="instance">The instance to populate.</param>
    /// <param name="ruleSets">An optional list of delimited rule sets to use for the populate request.</param>
    public override void Populate(TType instance, string ruleSets = null)
    {
        AutoGenerateContext context = CreateContext(ruleSets);
        PrepareFinish(context);

        base.Populate(instance, ruleSets);
    }

    private AutoGenerateContext CreateContext(string ruleSets)
    {
        var config = new AutoConfig(_config ?? AutoFaker.DefaultConfig);

        if (!string.IsNullOrWhiteSpace(Locale))
        {
            config.Locale = Locale;
        }

        if (Binder != null)
        {
            config.Binder = Binder;
        }

        return new AutoGenerateContext(FakerHub, config)
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
    private List<string> ParseRuleSets(string? ruleSets)
    {
        if (string.IsNullOrWhiteSpace(ruleSets))
        {
            ruleSets = null;
        }

        List<string> validRuleSets = new List<string>();

        string[] ruleSetArray = ruleSets?.Split(',') ?? new[] { currentRuleSet };

        for (int i = 0; i < ruleSetArray.Length; i++)
        {
            string trimmedRuleSet = ruleSetArray[i].Trim();

            if (!string.IsNullOrWhiteSpace(trimmedRuleSet))
            {
                validRuleSets.Add(trimmedRuleSet);
            }
        }

        return validRuleSets;
    }

    private void PrepareCreate(AutoGenerateContext context)
    {
        // Check a create handler hasn't previously been set or configured externally
        if (!CreateInitialized && CreateActions[currentRuleSet] == null)
        {
            CreateActions[currentRuleSet] = faker =>
            {
                // Only auto create if the 'default' rule set is defined for generation
                // This is because any specific rule sets are expected to handle the full creation
                if (context.RuleSets.Contains(currentRuleSet))
                {
                    Type type = typeof(TType);

                    // Set the current type being generated
                    context.ParentType = null;
                    context.GenerateType = type;
                    context.GenerateName = null;

                    // Get the members that should not be set during construction
                    List<string> memberNames = GetRuleSetsMemberNames(context);

                    foreach (string? memberName in TypeProperties.Keys)
                    {
                        if (memberNames.Contains(memberName))
                        {
                            var path = $"{type.FullName}.{memberName}";
                            bool existing = context.Config.SkipPaths.Any(s => s == path);

                            if (!existing)
                            {
                                context.Config.SkipPaths.Add(path);
                            }
                        }
                    }

                    // Create a blank instance. It will be populated in the FinalizeAction registered
                    // by PrepareFinish (context.Binder.PopulateInstance<TType>).
                    return context.Binder.CreateInstance<TType>(context);
                }

                return DefaultCreateAction.Invoke(faker);
            };

            CreateInitialized = true;
        }
    }

    private void PrepareFinish(AutoGenerateContext context)
    {
        if (FinishInitialized)
            return;

        FinalizeActions.TryGetValue(currentRuleSet, out FinalizeAction<TType> finishWith);

        FinishWith((faker, instance) =>
        {
            if (instance is not null && instance.GetType().IsExpandoObject())
            {
                context.ParentType = null;
                context.GenerateType = instance.GetType();
                context.GenerateName = null;
                context.Instance = instance;

                IAutoGenerator generator = AutoGeneratorFactory.GetGenerator(context);
                generator.Generate(context);

                context.Instance = null;
                return;
            }

            List<string> memberNames = GetRuleSetsMemberNames(context);

            var members = new MemberInfo[TypeProperties.Count - memberNames.Count];
            int index = 0;

            foreach (var member in TypeProperties)
            {
                if (!memberNames.Contains(member.Key))
                {
                    members[index++] = member.Value;
                }
            }

            context.Binder.PopulateInstance<TType>(instance, context, members.Where(m => m != null).ToArray());

            finishWith?.Action(faker, instance);
        });

        FinishInitialized = true;
    }

    private List<string> GetRuleSetsMemberNames(AutoGenerateContext context)
    {
        var members = new List<string>();

        for (int i = 0; i < context.RuleSets.Count; i++)
        {
            string? ruleSetName = context.RuleSets[i];
            if (Actions.TryGetValue(ruleSetName, out var ruleSet))
            {
                foreach (var keyValuePair in ruleSet)
                {
                    members.Add(keyValuePair.Key);
                }
            }
        }

        return members;
    }
}