public interface IWorkerPrioritizable
{
	bool GetWorkerPriority(Worker worker, out int priority);
}
