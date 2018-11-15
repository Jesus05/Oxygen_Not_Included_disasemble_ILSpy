using System;
using UnityEngine;

public struct Matrix2x3
{
	public float m00;

	public float m01;

	public float m02;

	public float m10;

	public float m11;

	public float m12;

	public static readonly Matrix2x3 identity = new Matrix2x3(1f, 0f, 0f, 0f, 1f, 0f);

	public Matrix2x3(float e00, float e01, float e02, float e10, float e11, float e12)
	{
		m00 = e00;
		m01 = e01;
		m02 = e02;
		m10 = e10;
		m11 = e11;
		m12 = e12;
	}

	public override bool Equals(object obj)
	{
		Matrix2x3 n = (Matrix2x3)obj;
		return this == n;
	}

	public override int GetHashCode()
	{
		return ((ValueType)this).GetHashCode();
	}

	public static Vector3 operator *(Matrix2x3 m, Vector3 v)
	{
		return new Vector3(v.x * m.m00 + v.y * m.m01 + m.m02, v.x * m.m10 + v.y * m.m11 + m.m12, v.z);
	}

	public static Matrix2x3 operator *(Matrix2x3 m, Matrix2x3 n)
	{
		return new Matrix2x3(m.m00 * n.m00 + m.m01 * n.m10, m.m00 * n.m01 + m.m01 * n.m11, m.m00 * n.m02 + m.m01 * n.m12 + m.m02 * 1f, m.m10 * n.m00 + m.m11 * n.m10, m.m10 * n.m01 + m.m11 * n.m11, m.m10 * n.m02 + m.m11 * n.m12 + m.m12 * 1f);
	}

	public static bool operator ==(Matrix2x3 m, Matrix2x3 n)
	{
		return m.m00 == n.m00 && m.m01 == n.m01 && m.m02 == n.m02 && m.m10 == n.m10 && m.m11 == n.m11 && m.m11 == n.m12;
	}

	public static bool operator !=(Matrix2x3 m, Matrix2x3 n)
	{
		return !(m == n);
	}

	public Vector3 MultiplyPoint(Vector3 v)
	{
		return new Vector3(v.x * m00 + v.y * m01 + m02, v.x * m10 + v.y * m11 + m12, v.z);
	}

	public Vector3 MultiplyVector(Vector3 v)
	{
		return new Vector3(v.x * m00 + v.y * m01, v.x * m10 + v.y * m11, v.z);
	}

	public static implicit operator Matrix4x4(Matrix2x3 m)
	{
		Matrix4x4 result = Matrix4x4.identity;
		result.m00 = m.m00;
		result.m01 = m.m01;
		result.m03 = m.m02;
		result.m10 = m.m10;
		result.m11 = m.m11;
		result.m13 = m.m12;
		return result;
	}

	public static Matrix2x3 Scale(Vector2 scale)
	{
		Matrix2x3 result = identity;
		result.m00 = scale.x;
		result.m11 = scale.y;
		return result;
	}

	public static Matrix2x3 Translate(Vector2 translation)
	{
		Matrix2x3 result = identity;
		result.m02 = translation.x;
		result.m12 = translation.y;
		return result;
	}

	public static Matrix2x3 Rotate(float angle_in_radians)
	{
		Matrix2x3 result = identity;
		float num = Mathf.Cos(angle_in_radians);
		float num2 = Mathf.Sin(angle_in_radians);
		result.m00 = num;
		result.m01 = 0f - num2;
		result.m10 = num2;
		result.m11 = num;
		return result;
	}

	public static Matrix2x3 Rotate(Quaternion quaternion)
	{
		Matrix2x3 result = identity;
		float num = quaternion.x * quaternion.x;
		float num2 = quaternion.y * quaternion.y;
		float num3 = quaternion.z * quaternion.z;
		float num4 = quaternion.x * quaternion.y;
		float num5 = quaternion.x * quaternion.z;
		float num6 = quaternion.y * quaternion.z;
		float num7 = quaternion.w * quaternion.x;
		float num8 = quaternion.w * quaternion.y;
		float num9 = quaternion.w * quaternion.z;
		result.m00 = 1f - 2f * (num2 + num3);
		result.m01 = 2f * (num4 - num9);
		result.m02 = 2f * (num5 + num8);
		result.m10 = 2f * (num4 + num9);
		result.m11 = 1f - 2f * (num + num3);
		result.m12 = 2f * (num6 - num7);
		return result;
	}

	public static Matrix2x3 TRS(Vector2 translation, Quaternion quaternion, Vector2 scale)
	{
		Matrix2x3 result = Rotate(quaternion);
		result.m00 *= scale.x;
		result.m11 *= scale.y;
		result.m02 = translation.x;
		result.m12 = translation.y;
		return result;
	}

	public static Matrix2x3 TRS(Vector2 translation, float angle_in_radians, Vector2 scale)
	{
		Matrix2x3 result = Rotate(angle_in_radians);
		result.m00 *= scale.x;
		result.m11 *= scale.y;
		result.m02 = translation.x;
		result.m12 = translation.y;
		return result;
	}

	public override string ToString()
	{
		return $"[{m00}, {m01}, {m02}]  [{m10}, {m11}, {m12}]";
	}
}
