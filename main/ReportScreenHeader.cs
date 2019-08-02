using UnityEngine;

public class ReportScreenHeader : KMonoBehaviour
{
	[SerializeField]
	private ReportScreenHeaderRow rowTemplate;

	private ReportScreenHeaderRow mainRow;

	public void SetMainEntry(ReportManager.ReportGroup reportGroup)
	{
		if ((Object)mainRow == (Object)null)
		{
			mainRow = Util.KInstantiateUI(rowTemplate.gameObject, base.gameObject, true).GetComponent<ReportScreenHeaderRow>();
		}
		mainRow.SetLine(reportGroup);
	}
}
