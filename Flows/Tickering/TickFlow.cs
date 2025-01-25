using Cleipnir.Flows;
using Cleipnir.ResilientFunctions.Domain;

namespace cBrain.Flows.Tickering;

[GenerateFlows]
public class TickFlow : Flow<string>
{
    public override async Task Run(string param)
    {
        var state = await Workflow.States.CreateOrGetDefault<State>();
                
        while (true)
        {
            await Task.Delay(1_000);
            Console.WriteLine($"[{param}]: #{state.I} ticked...");
            state.I++;
            await state.Save();
        }
    }

    private class State : FlowState
    {
        public int I { get; set; }
    }
}