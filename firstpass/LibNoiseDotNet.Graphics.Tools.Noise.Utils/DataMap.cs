using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Utils
{
	public abstract class DataMap<T>
	{
		protected T _borderValue;

		protected int _width = 0;

		protected int _height = 0;

		private int _stride = 0;

		protected int _memoryUsage = 0;

		protected int _cellsCount = 0;

		protected T[] _data = null;

		protected bool _hasMaxDimension = false;

		protected int _maxWidth = 0;

		protected int _maxHeight = 0;

		public int Width => _width;

		public int Height => _height;

		public int Stride
		{
			get
			{
				return _stride;
			}
			set
			{
				_stride = value;
			}
		}

		public T BorderValue
		{
			get
			{
				return _borderValue;
			}
			set
			{
				_borderValue = value;
			}
		}

		public int MemoryUsage => _memoryUsage;

		public float MemoryUsageKb => (float)MemoryUsage / 8192f;

		public float MemoryUsageMo => (float)MemoryUsage / 8388608f;

		public DataMap()
		{
			AllocateBuffer();
		}

		public DataMap(int width, int height)
		{
			AllocateBuffer(width, height);
		}

		public DataMap(DataMap<T> copy)
		{
			CopyFrom(copy);
		}

		public T[] GetSlab(int y)
		{
			T[] array = new T[_stride];
			if (_data != null && y >= 0 && y < _height)
			{
				Array.Copy(_data, y * _stride, array, 0, _stride);
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _borderValue;
				}
			}
			return array;
		}

		public T GetValue(int x, int y)
		{
			if (_data != null && x >= 0 && x < _width && y >= 0 && y < _height)
			{
				return _data[y * _stride + x];
			}
			return _borderValue;
		}

		public void SetValue(int x, int y, T value)
		{
			if (_data != null && x >= 0 && x < _width && y >= 0 && y < _height)
			{
				_data[y * _stride + x] = value;
			}
		}

		public void SetSize(int width, int height)
		{
			if (width < 0 || height < 0)
			{
				throw new ArgumentException("Map dimension must be greater or equal 0");
			}
			if (_hasMaxDimension && (width > _maxWidth || height > _maxHeight))
			{
				throw new ArgumentException($"Map dimension must be lower than {_maxWidth} * {_maxHeight}");
			}
			AllocateBuffer(width, height);
		}

		public void CopyFrom(DataMap<T> source)
		{
			AllocateBuffer(source._width, source._height);
			if (_cellsCount > 0)
			{
				Array.Copy(source._data, 0, _data, 0, _cellsCount);
			}
			_borderValue = source._borderValue;
		}

		public void CopyTo(DataMap<T> dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("Dest is null");
			}
			dest.CopyFrom(this);
		}

		public void CopyTo(ref T[] buffer)
		{
			if (_data != null)
			{
				if (buffer == null)
				{
					buffer = new T[_cellsCount];
				}
				int length = (_data.Length <= buffer.Length) ? _data.Length : buffer.Length;
				Array.Copy(_data, 0, buffer, 0, length);
			}
		}

		public T[] Share()
		{
			if (_data == null)
			{
				throw new NullReferenceException("The internal buffer is null");
			}
			return _data;
		}

		public void Reset()
		{
			AllocateBuffer(0, 0);
		}

		public void DeleteAndReset()
		{
			_data = null;
			AllocateBuffer(0, 0);
		}

		public void ReclaimMemory()
		{
			if (_data != null && _data.Length > _cellsCount)
			{
				Array.Resize<T>(ref _data, _cellsCount);
			}
		}

		public void Clear(T value)
		{
			if (_data != null)
			{
				for (int i = 0; i <= _cellsCount; i++)
				{
					_data[i] = value;
				}
			}
		}

		public void Clear()
		{
			if (_data != null)
			{
				Array.Clear(_data, 0, _cellsCount);
			}
		}

		protected abstract int SizeofT();

		protected abstract T MinvalofT();

		protected abstract T MaxvalofT();

		protected void AllocateBuffer()
		{
			_cellsCount = _width * _height;
			_stride = _width;
			_memoryUsage = _cellsCount * SizeofT();
			if (_cellsCount != 0)
			{
				if (_data == null)
				{
					_data = new T[_cellsCount];
				}
				else if (_data.Length < _cellsCount)
				{
					Array.Resize<T>(ref _data, _cellsCount);
				}
			}
		}

		protected void AllocateBuffer(int width, int height)
		{
			_width = width;
			_height = height;
			AllocateBuffer();
		}
	}
}
