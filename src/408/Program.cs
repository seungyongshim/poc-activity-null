using System.Diagnostics;
using System.Reflection;

#region setup
Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name);

using var listener = new ActivityListener
{
    ShouldListenTo = _ => true,
    Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
    ActivityStarted = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Start"),
    ActivityStopped = activity => Console.WriteLine($"{activity.ParentId}:{activity.Id} - Stop")
};

ActivitySource.AddActivityListener(listener);
var activitySource = new ActivitySource("what?!");
#endregion

var q = from _1 in unitAff
        from _2 in use(from _a in unitAff
                       let _b = activitySource.StartActivity()
                       select _b,
                       a => Eff(() => Activity.Current ?? throw new Exception("Bug?")).ToAsync())
        select unit;

var r = await q.Run();
r.ThrowIfFail();


