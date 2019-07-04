using System.Collections.Generic;

public interface IGroupProber
{
	void Occupy(object prober, int serial_no, IEnumerable<int> cells);

	void SetValidSerialNos(object prober, int previous_serial_no, int serial_no);

	bool ReleaseProber(object prober);
}
