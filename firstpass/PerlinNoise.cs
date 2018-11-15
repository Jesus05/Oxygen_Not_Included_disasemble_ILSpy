using System;

public class PerlinNoise
{
	private const int GradientSizeTable = 256;

	private readonly Random _random;

	private readonly double[] _gradients = new double[768];

	private readonly byte[] _perm = new byte[256]
	{
		225,
		155,
		210,
		108,
		175,
		199,
		221,
		144,
		203,
		116,
		70,
		213,
		69,
		158,
		33,
		252,
		5,
		82,
		173,
		133,
		222,
		139,
		174,
		27,
		9,
		71,
		90,
		246,
		75,
		130,
		91,
		191,
		169,
		138,
		2,
		151,
		194,
		235,
		81,
		7,
		25,
		113,
		228,
		159,
		205,
		253,
		134,
		142,
		248,
		65,
		224,
		217,
		22,
		121,
		229,
		63,
		89,
		103,
		96,
		104,
		156,
		17,
		201,
		129,
		36,
		8,
		165,
		110,
		237,
		117,
		231,
		56,
		132,
		211,
		152,
		20,
		181,
		111,
		239,
		218,
		170,
		163,
		51,
		172,
		157,
		47,
		80,
		212,
		176,
		250,
		87,
		49,
		99,
		242,
		136,
		189,
		162,
		115,
		44,
		43,
		124,
		94,
		150,
		16,
		141,
		247,
		32,
		10,
		198,
		223,
		byte.MaxValue,
		72,
		53,
		131,
		84,
		57,
		220,
		197,
		58,
		50,
		208,
		11,
		241,
		28,
		3,
		192,
		62,
		202,
		18,
		215,
		153,
		24,
		76,
		41,
		15,
		179,
		39,
		46,
		55,
		6,
		128,
		167,
		23,
		188,
		106,
		34,
		187,
		140,
		164,
		73,
		112,
		182,
		244,
		195,
		227,
		13,
		35,
		77,
		196,
		185,
		26,
		200,
		226,
		119,
		31,
		123,
		168,
		125,
		249,
		68,
		183,
		230,
		177,
		135,
		160,
		180,
		12,
		1,
		243,
		148,
		102,
		166,
		38,
		238,
		251,
		37,
		240,
		126,
		64,
		74,
		161,
		40,
		184,
		149,
		171,
		178,
		101,
		66,
		29,
		59,
		146,
		61,
		254,
		107,
		42,
		86,
		154,
		4,
		236,
		232,
		120,
		21,
		233,
		209,
		45,
		98,
		193,
		114,
		78,
		19,
		206,
		14,
		118,
		127,
		48,
		79,
		147,
		85,
		30,
		207,
		219,
		54,
		88,
		234,
		190,
		122,
		95,
		67,
		143,
		109,
		137,
		214,
		145,
		93,
		92,
		100,
		245,
		0,
		216,
		186,
		60,
		83,
		105,
		97,
		204,
		52
	};

	public PerlinNoise(int seed)
	{
		_random = new Random(seed);
		InitGradients();
	}

	public double Noise(double x, double y, double z)
	{
		int num = (int)Math.Floor(x);
		double num2 = x - (double)num;
		double fx = num2 - 1.0;
		double t = Smooth(num2);
		int num3 = (int)Math.Floor(y);
		double num4 = y - (double)num3;
		double fy = num4 - 1.0;
		double t2 = Smooth(num4);
		int num5 = (int)Math.Floor(z);
		double num6 = z - (double)num5;
		double fz = num6 - 1.0;
		double t3 = Smooth(num6);
		double value = Lattice(num, num3, num5, num2, num4, num6);
		double value2 = Lattice(num + 1, num3, num5, fx, num4, num6);
		double value3 = Lerp(t, value, value2);
		value = Lattice(num, num3 + 1, num5, num2, fy, num6);
		value2 = Lattice(num + 1, num3 + 1, num5, fx, fy, num6);
		double value4 = Lerp(t, value, value2);
		double value5 = Lerp(t2, value3, value4);
		value = Lattice(num, num3, num5 + 1, num2, num4, fz);
		value2 = Lattice(num + 1, num3, num5 + 1, fx, num4, fz);
		value3 = Lerp(t, value, value2);
		value = Lattice(num, num3 + 1, num5 + 1, num2, fy, fz);
		value2 = Lattice(num + 1, num3 + 1, num5 + 1, fx, fy, fz);
		value4 = Lerp(t, value, value2);
		double value6 = Lerp(t2, value3, value4);
		return Lerp(t3, value5, value6);
	}

	private void InitGradients()
	{
		for (int i = 0; i < 256; i++)
		{
			double num = 1.0 - 2.0 * _random.NextDouble();
			double num2 = Math.Sqrt(1.0 - num * num);
			double num3 = 6.2831853071795862 * _random.NextDouble();
			_gradients[i * 3] = num2 * Math.Cos(num3);
			_gradients[i * 3 + 1] = num2 * Math.Sin(num3);
			_gradients[i * 3 + 2] = num;
		}
	}

	private int Permutate(int x)
	{
		return _perm[x & 0xFF];
	}

	private int Index(int ix, int iy, int iz)
	{
		return Permutate(ix + Permutate(iy + Permutate(iz)));
	}

	private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
	{
		int num = Index(ix, iy, iz);
		int num2 = num * 3;
		return _gradients[num2] * fx + _gradients[num2 + 1] * fy + _gradients[num2 + 2] * fz;
	}

	private double Lerp(double t, double value0, double value1)
	{
		return value0 + t * (value1 - value0);
	}

	private double Smooth(double x)
	{
		return x * x * (3.0 - 2.0 * x);
	}
}
