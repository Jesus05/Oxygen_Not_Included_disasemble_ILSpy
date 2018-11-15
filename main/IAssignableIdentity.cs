using System.Collections.Generic;

public interface IAssignableIdentity
{
	string GetProperName();

	List<Ownables> GetOwners();

	Ownables GetSoleOwner();

	bool IsNull();
}
