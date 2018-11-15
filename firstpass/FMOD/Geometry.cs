using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Geometry
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_Geometry_Release(handle);
		}

		public RESULT addPolygon(float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex)
		{
			return FMOD5_Geometry_AddPolygon(handle, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
		}

		public RESULT getNumPolygons(out int numpolygons)
		{
			return FMOD5_Geometry_GetNumPolygons(handle, out numpolygons);
		}

		public RESULT getMaxPolygons(out int maxpolygons, out int maxvertices)
		{
			return FMOD5_Geometry_GetMaxPolygons(handle, out maxpolygons, out maxvertices);
		}

		public RESULT getPolygonNumVertices(int index, out int numvertices)
		{
			return FMOD5_Geometry_GetPolygonNumVertices(handle, index, out numvertices);
		}

		public RESULT setPolygonVertex(int index, int vertexindex, ref VECTOR vertex)
		{
			return FMOD5_Geometry_SetPolygonVertex(handle, index, vertexindex, ref vertex);
		}

		public RESULT getPolygonVertex(int index, int vertexindex, out VECTOR vertex)
		{
			return FMOD5_Geometry_GetPolygonVertex(handle, index, vertexindex, out vertex);
		}

		public RESULT setPolygonAttributes(int index, float directocclusion, float reverbocclusion, bool doublesided)
		{
			return FMOD5_Geometry_SetPolygonAttributes(handle, index, directocclusion, reverbocclusion, doublesided);
		}

		public RESULT getPolygonAttributes(int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
		{
			return FMOD5_Geometry_GetPolygonAttributes(handle, index, out directocclusion, out reverbocclusion, out doublesided);
		}

		public RESULT setActive(bool active)
		{
			return FMOD5_Geometry_SetActive(handle, active);
		}

		public RESULT getActive(out bool active)
		{
			return FMOD5_Geometry_GetActive(handle, out active);
		}

		public RESULT setRotation(ref VECTOR forward, ref VECTOR up)
		{
			return FMOD5_Geometry_SetRotation(handle, ref forward, ref up);
		}

		public RESULT getRotation(out VECTOR forward, out VECTOR up)
		{
			return FMOD5_Geometry_GetRotation(handle, out forward, out up);
		}

		public RESULT setPosition(ref VECTOR position)
		{
			return FMOD5_Geometry_SetPosition(handle, ref position);
		}

		public RESULT getPosition(out VECTOR position)
		{
			return FMOD5_Geometry_GetPosition(handle, out position);
		}

		public RESULT setScale(ref VECTOR scale)
		{
			return FMOD5_Geometry_SetScale(handle, ref scale);
		}

		public RESULT getScale(out VECTOR scale)
		{
			return FMOD5_Geometry_GetScale(handle, out scale);
		}

		public RESULT save(IntPtr data, out int datasize)
		{
			return FMOD5_Geometry_Save(handle, data, out datasize);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_Geometry_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_Geometry_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_Release(IntPtr geometry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetNumPolygons(IntPtr geometry, out int numpolygons);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, ref VECTOR vertex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out VECTOR vertex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetActive(IntPtr geometry, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetActive(IntPtr geometry, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetRotation(IntPtr geometry, ref VECTOR forward, ref VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetRotation(IntPtr geometry, out VECTOR forward, out VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetPosition(IntPtr geometry, ref VECTOR position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetPosition(IntPtr geometry, out VECTOR position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetScale(IntPtr geometry, ref VECTOR scale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetScale(IntPtr geometry, out VECTOR scale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_Save(IntPtr geometry, IntPtr data, out int datasize);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Geometry_GetUserData(IntPtr geometry, out IntPtr userdata);

		public bool hasHandle()
		{
			return handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			handle = IntPtr.Zero;
		}
	}
}
