public interface IComponentManager
{
	int Count
	{
		get;
	}

	string Name
	{
		get;
	}

	void Spawn();

	void RenderEveryTick(float dt);

	void FixedUpdate(float dt);

	void Sim200ms(float dt);

	void CleanUp();

	void Clear();

	bool Has(object go);
}
