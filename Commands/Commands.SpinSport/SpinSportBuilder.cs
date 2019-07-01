using BeaverSoft.Texo.Core.Configuration;

namespace Commands.SpinSport
{
    public static class SpinSportBuilder
    {
        public static Query BuildCommand()
        {
            var command = Query.CreateBuilder();
            command.Key = SpinSportConstants.COMMAND_NAME;
            command.DefaultQueryKey = SpinSportConstants.QUERY_CONFIG_COLOUR;
            command.Representations.AddRange(
                new[] { SpinSportConstants.COMMAND_NAME, "spin", "sport", "ss", "client" });
            command.Documentation.Title = "spin-sport tools";
            command.Documentation.Description = "Support commands for SpinSport client development.";

            command.Queries.AddRange(
                new[]
                {
                    BuildColourQuery(),
                    BuildColourUsageQuery(),
                    BuildLocalisationQuery(),
                });

            return command.ToImmutable();
        }

        private static Query BuildColourQuery()
        {
            var colour = Query.CreateBuilder();
            colour.Key = SpinSportConstants.QUERY_CONFIG_COLOUR;
            colour.DefaultQueryKey = SpinSportConstants.QUERY_LIST;
            colour.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_CONFIG_COLOUR, "color", "c" });
            colour.Documentation.Title = "config-colour";
            colour.Documentation.Description = "Configuration of colours.";

            var list = Query.CreateBuilder();
            list.Key = SpinSportConstants.QUERY_LIST;
            list.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_LIST, "get" });
            list.Documentation.Title = "config-colour-list";
            list.Documentation.Description = "Gets a list of configured colours.";

            var parameterFilter = Parameter.CreateBuilder();
            parameterFilter.Key = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.IsOptional = true;
            parameterFilter.ArgumentTemplate = @"^[A-Za-z\d_\*]+$";
            parameterFilter.Documentation.Title = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.Documentation.Description = "Filter for a list.";
            list.Parameters.Add(parameterFilter.ToImmutable());

            var set = Query.CreateBuilder();
            set.Key = SpinSportConstants.QUERY_SET;
            set.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_SET });
            set.Documentation.Title = "config-colour-set";
            set.Documentation.Description = "Sets or updates a colour.";

            var parameterName = Parameter.CreateBuilder();
            parameterName.Key = SpinSportConstants.PARAMETER_NAME;
            parameterName.ArgumentTemplate = @"^[A-Za-z\d_]+$";
            parameterName.Documentation.Title = "colour-name";
            parameterName.Documentation.Description = "Name of the colour.";
            set.Parameters.Add(parameterName.ToImmutable());

            var parameterValue = Parameter.CreateBuilder();
            parameterValue.Key = SpinSportConstants.PARAMETER_VALUE;
            parameterValue.Documentation.Title = "colour-value";
            parameterValue.Documentation.Description = "Definition of the color.";
            set.Parameters.Add(parameterValue.ToImmutable());

            colour.Queries.AddRange(
                new[]
                {
                    list.ToImmutable(),
                    set.ToImmutable(),
                });

            return colour.ToImmutable();
        }

        private static Query BuildColourUsageQuery()
        {
            var colourUsage = Query.CreateBuilder();
            colourUsage.Key = SpinSportConstants.QUERY_CONFIG_COLOUR_USAGE;
            colourUsage.DefaultQueryKey = SpinSportConstants.QUERY_LIST;
            colourUsage.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_CONFIG_COLOUR_USAGE, "cu" });
            colourUsage.Documentation.Title = "config-colour-usage";
            colourUsage.Documentation.Description = "Configuration of colour usages.";

            var parameterFilter = Parameter.CreateBuilder();
            parameterFilter.Key = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.IsOptional = true;
            parameterFilter.ArgumentTemplate = @"^[A-Za-z\d_\*]+$";
            parameterFilter.Documentation.Title = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.Documentation.Description = "Filter for a list.";

            var list = Query.CreateBuilder();
            list.Key = SpinSportConstants.QUERY_LIST;
            list.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_LIST });
            list.Documentation.Title = "config-colour-usage-list";
            list.Documentation.Description = "Gets a list of configured colour usages.";
            list.Parameters.Add(parameterFilter.ToImmutable());

            var get = Query.CreateBuilder();
            get.Key = SpinSportConstants.QUERY_GET;
            get.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_GET });
            get.Documentation.Title = "config-colour-usage-get";
            get.Documentation.Description = "Gets a detail of colour usage(s).";
            get.Parameters.Add(parameterFilter.ToImmutable());

            var set = Query.CreateBuilder();
            set.Key = SpinSportConstants.QUERY_SET;
            set.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_SET });
            set.Documentation.Title = "config-colour-usage-set";
            set.Documentation.Description = "Sets or updates a colour usage.";

            var parameterName = Parameter.CreateBuilder();
            parameterName.Key = SpinSportConstants.PARAMETER_NAME;
            parameterName.ArgumentTemplate = @"^[A-Za-z\d_-.]+$";
            parameterName.Documentation.Title = "colour-usage-name";
            parameterName.Documentation.Description = "Name of the colour usage.";
            set.Parameters.Add(parameterName.ToImmutable());

            var parameterValue = Parameter.CreateBuilder();
            parameterValue.Key = SpinSportConstants.PARAMETER_VALUE;
            parameterValue.ArgumentTemplate = @"^(\$|\£)[A-Za-z\d_]+$";
            parameterValue.Documentation.Title = "colour-usage-value";
            parameterValue.Documentation.Description = "Definition of the color usage.";

            var optionShowcase = Option.CreateBuilder();
            optionShowcase.Key = SpinSportConstants.OPTION_SHOWCASE;
            optionShowcase.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_SHOWCASE, "s" });
            optionShowcase.Parameters.Add(parameterValue.ToImmutable());
            optionShowcase.Documentation.Title = SpinSportConstants.OPTION_SHOWCASE;
            optionShowcase.Documentation.Description = "Colour usage definition for showcase brand.";
            set.Options.Add(optionShowcase.ToImmutable());

            var optionBetway = Option.CreateBuilder();
            optionBetway.Key = SpinSportConstants.OPTION_BETWAY;
            optionBetway.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_BETWAY, "b" });
            optionBetway.Parameters.Add(parameterValue.ToImmutable());
            optionBetway.Documentation.Title = SpinSportConstants.OPTION_BETWAY;
            optionBetway.Documentation.Description = "Colour usage definition for betway brand.";
            set.Options.Add(optionBetway.ToImmutable());

            var optionBetwayDark = Option.CreateBuilder();
            optionBetwayDark.Key = SpinSportConstants.OPTION_BETWAY_DARK;
            optionBetwayDark.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_BETWAY_DARK, "d" });
            optionBetwayDark.Parameters.Add(parameterValue.ToImmutable());
            optionBetwayDark.Documentation.Title = SpinSportConstants.OPTION_BETWAY_DARK;
            optionBetwayDark.Documentation.Description = "Colour usage definition for betway-dark brand.";
            set.Options.Add(optionBetwayDark.ToImmutable());

            var optionBetwayNew = Option.CreateBuilder();
            optionBetwayNew.Key = SpinSportConstants.OPTION_BETWAY_NEW;
            optionBetwayNew.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_BETWAY_NEW, "n" });
            optionBetwayNew.Parameters.Add(parameterValue.ToImmutable());
            optionBetwayNew.Documentation.Title = SpinSportConstants.OPTION_BETWAY_NEW;
            optionBetwayNew.Documentation.Description = "Colour usage definition for betway-new brand.";
            set.Options.Add(optionBetwayNew.ToImmutable());

            var optionBetwayAfrica = Option.CreateBuilder();
            optionBetwayAfrica.Key = SpinSportConstants.OPTION_BETWAY_AFRICA;
            optionBetwayAfrica.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_BETWAY_AFRICA, "a" });
            optionBetwayAfrica.Parameters.Add(parameterValue.ToImmutable());
            optionBetwayAfrica.Documentation.Title = SpinSportConstants.OPTION_BETWAY_AFRICA;
            optionBetwayAfrica.Documentation.Description = "Colour usage definition for betway-africa brand.";
            set.Options.Add(optionBetwayAfrica.ToImmutable());

            var optionBetwayAfricaMobile = Option.CreateBuilder();
            optionBetwayAfricaMobile.Key = SpinSportConstants.OPTION_BETWAY_AFRICA_MOBILE;
            optionBetwayAfricaMobile.Representations.AddRange(
                new[] { SpinSportConstants.OPTION_BETWAY_AFRICA_MOBILE, "m" });
            optionBetwayAfricaMobile.Parameters.Add(parameterValue.ToImmutable());
            optionBetwayAfricaMobile.Documentation.Title = SpinSportConstants.OPTION_BETWAY_AFRICA_MOBILE;
            optionBetwayAfricaMobile.Documentation.Description = "Colour usage definition for betway-africa-mobile brand.";
            set.Options.Add(optionBetwayAfricaMobile.ToImmutable());

            colourUsage.Queries.AddRange(
                new[]
                {
                    list.ToImmutable(),
                    get.ToImmutable(),
                    set.ToImmutable(),
                });

            return colourUsage.ToImmutable();
        }

        private static Query BuildLocalisationQuery()
        {
            var colourUsage = Query.CreateBuilder();
            colourUsage.Key = SpinSportConstants.QUERY_CONFIG_LOCALISATION;
            colourUsage.DefaultQueryKey = SpinSportConstants.QUERY_LIST;
            colourUsage.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_CONFIG_LOCALISATION, "lang" });
            colourUsage.Documentation.Title = "config-localisation";
            colourUsage.Documentation.Description = "Localisation for SpinSport application.";

            var parameterFilter = Parameter.CreateBuilder();
            parameterFilter.Key = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.IsOptional = true;
            parameterFilter.ArgumentTemplate = @"^[A-Za-z\d_\*]+$";
            parameterFilter.Documentation.Title = SpinSportConstants.PARAMETER_FILTER;
            parameterFilter.Documentation.Description = "Filter for a list.";

            var list = Query.CreateBuilder();
            list.Key = SpinSportConstants.QUERY_LIST;
            list.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_LIST });
            list.Documentation.Title = "config-localisation-list";
            list.Documentation.Description = "Gets a list of localisation variables.";
            list.Parameters.Add(parameterFilter.ToImmutable());

            var get = Query.CreateBuilder();
            get.Key = SpinSportConstants.QUERY_GET;
            get.Representations.AddRange(
                new[] { SpinSportConstants.QUERY_GET });
            get.Documentation.Title = "config-localisation-get";
            get.Documentation.Description = "Gets a detail of localisation variable(s).";
            get.Parameters.Add(parameterFilter.ToImmutable());

            colourUsage.Queries.AddRange(
                new[]
                {
                    list.ToImmutable(),
                    get.ToImmutable()
                });

            return colourUsage.ToImmutable();
        }
    }
}
