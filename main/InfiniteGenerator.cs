public class InfiniteGenerator : Generator
{
	public override bool IsEmpty => false;

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ApplyDeltaJoules(base.WattageRating * dt, false);
	}
}
