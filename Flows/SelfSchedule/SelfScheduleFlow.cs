using Cleipnir.Flows;

namespace cBrain.Flows.SelfSchedule;

[GenerateFlows]
public class SelfScheduleFlow(SelfScheduleFlows flows) : Flow<int>
{
    
    public override async Task Run(int param)
    {
        await DoWork();

        await flows.ScheduleIn($"InstanceId#{param + 1}", param + 1, TimeSpan.FromSeconds(5));
    }

    private Task DoWork() => Task.Delay(100);
}