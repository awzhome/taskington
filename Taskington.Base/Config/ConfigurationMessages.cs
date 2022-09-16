using System.Collections.Generic;
using Taskington.Base.Plans;
using Taskington.Base.TinyBus;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.Config;

public record InitializeConfigurationMessage : Message<InitializeConfigurationMessage>;
public record ConfigurationChangedMessage : Message<ConfigurationChangedMessage>;
public record ConfigurationReloadedMessage : Message<ConfigurationReloadedMessage>;
public record ConfigurationReloadDelayedMessage : Message<ConfigurationReloadDelayedMessage>;

public record GetConfigValueMessage(string Key) : RequestMessage<GetConfigValueMessage, string?>;
public record SetConfigValueMessage(string Key, string Value) : Message<SetConfigValueMessage>;
public record SaveConfigurationMessage : Message<SaveConfigurationMessage>;
public record InsertPlanMessage(int Index, Plan Plan) : Message<InsertPlanMessage>;
public record RemovePlanMessage(Plan Plan) : Message<RemovePlanMessage>;
public record ReplacePlanMessage(Plan OldPlan, Plan NewPlan) : Message<ReplacePlanMessage>;
public record GetPlansMessage : RequestMessage<GetPlansMessage, IEnumerable<Plan>>;
