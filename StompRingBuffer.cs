﻿using System;

namespace STOMP
{
    /// <summary>
    ///     A basic ring buffer class that allows you to write and read data, with limited seeking
    /// </summary>
    /// <typeparam name="T">
    ///     Any type that can be held in an array
    /// </typeparam>
    public class StompRingBuffer<T>
    {
        private T[] _Buffer;
        private int _WritePtr;
        private int _ReadPtr;
        private int _AmtWritten;
        private int _AmtRead;

        private int _SeekOffset;

        /// <summary>
        ///     Creates a new ringbuffer of the given size
        /// </summary>
        /// <param name="BufferSize">
        ///     The size, in elements, of the buffer
        /// </param>
        public StompRingBuffer(int BufferSize)
        {
            _Buffer = new T[BufferSize];
            _AmtWritten = 0;
        }

        /// <summary>
        ///     How many elements are available to write
        /// </summary>
        public int AvailableWrite { get { return _Buffer.Length - _AmtWritten + _AmtRead; } }

        /// <summary>
        ///     How any elements are available to read
        /// </summary>
        public int AvailableRead
        {
            get
            {
                return _AmtWritten - _AmtRead - _SeekOffset;
            }
        }

        /// <summary>
        ///     Write data to the buffer of the given length
        /// </summary>
        /// <param name="Data">
        ///     The T[] to write data into the buffer from, starting from offset 0
        /// </param>
        /// <param name="Length">
        ///     How much data to write to the ring buffer
        /// </param>
        public void Write(T[] Data, int Length)
        {
            if (AvailableWrite < Length)
            {
                throw new InvalidOperationException("Unable to add data to ring buffer - ring buffer full");
            }

            if (_WritePtr + Length > _Buffer.Length)
            {
                // Split write into two
                int Split = _Buffer.Length - _WritePtr;
                Array.Copy(Data, 0, _Buffer, _WritePtr, Split);
                Array.Copy(Data, Length - Split, _Buffer, 0, Length - Split);
            }
            else
            {
                // Single write
                Array.Copy(Data, 0, _Buffer, _WritePtr, Length);
            }

            _AmtWritten += Length;
            _WritePtr = (_WritePtr + Length) % _Buffer.Length;

            if (AvailableWrite < -_SeekOffset)
            {
                _SeekOffset = -AvailableWrite;
            }
        }

        /// <summary>
        ///     Write data to the buffer
        /// </summary>
        /// <param name="Data">
        ///     The data to write to the buffer.  All elements will be written
        /// </param>
        public void Write(T[] Data)
        {
            Write(Data, Data.Length);
        }

        /// <summary>
        ///     Seek by <i>Amount</i> through the ring buffer
        /// </summary>
        /// <param name="Amount">
        ///     How much to seek through the ring buffer by
        /// </param>
        /// <remarks>
        ///     <para>
        ///         Seek() allows you to seek through previously read data or skip ahead through written data without reading.
        ///     </para>
        ///     <para>
        ///         Note that seeking is not reliable in an active ring buffer and seeking forward may cause some data to be written over!
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The current seek position
        /// </returns>
        public int Seek(int Amount)
        {
            _SeekOffset += Amount;
            if (_SeekOffset > 0)
            {
                _SeekOffset = Math.Min(_SeekOffset, AvailableRead);

                _ReadPtr += _SeekOffset;
                _AmtRead += _SeekOffset;

                _SeekOffset = 0;
            }

            if (AvailableRead < -_SeekOffset)
            {
                _SeekOffset = -AvailableRead;
            }

            return _SeekOffset;
        }

        /// <summary>
        ///     Read data out of the ring buffer from the current seek position
        /// </summary>
        /// <param name="Amount">
        ///     How many buffer elements to read and return
        /// </param>
        /// <returns>
        ///     An array containing the next <i>Amount</i> elements in the buffer starting from the current seek position
        /// </returns>
        public T[] Read(int Amount)
        {
            if (Amount > _Buffer.Length - AvailableRead)
                throw new InvalidOperationException("Cannot read past end of ring");

            T[] Data = Peek(Amount);
            
            _SeekOffset += Amount;

            if (_SeekOffset > 0)
            {
                _AmtRead += _SeekOffset;
                _ReadPtr += _SeekOffset;
                _SeekOffset = 0;
            }

            return Data;
        }

        /// <summary>
        ///     Read data out of the ring buffer from the current seek position without consuming it from the buffer
        /// </summary>
        /// <param name="Amount">
        ///     How many buffer elements to read and return
        /// </param>
        /// <returns>
        ///     An array containing the next <i>Amount</i> elements in the buffer starting from the current seek position
        /// </returns>
        public T[] Peek(int Amount)
        {
            int ReadFrom = (_ReadPtr + _SeekOffset + _Buffer.Length) % _Buffer.Length;

            T[] Data = new T[Amount];

            if (ReadFrom + Amount >= _Buffer.Length)
            {
                int Split = _Buffer.Length - ReadFrom;

                Array.Copy(_Buffer, ReadFrom, Data, 0, Split);
                Array.Copy(_Buffer, 0, Data, Split, Amount - Split);
            }
            else
            {
                Array.Copy(_Buffer, ReadFrom, Data, 0, Amount);
            }

            return Data;
        }

        /// <summary>
        ///     Peeks at the current element pointed to by the seek offset without advancing
        /// </summary>
        /// <returns>
        ///     The current element pointed to by the seek offset
        /// </returns>
        public T Peek()
        {
            return _Buffer[(_ReadPtr + _SeekOffset + _Buffer.Length) % _Buffer.Length];
        }

        /// <summary>
        ///     How many elements lie between the current seek position and the first occurrence of the requested value.
        /// </summary>
        /// <param name="Value">
        ///     The value to search for
        /// </param>
        /// <remarks>
        ///     This function uses object.Equals() internally to determine equality.
        /// </remarks>
        /// <returns>
        ///     -1 if the value has not been written yet
        ///     0 or more if the value has been found.  Read() this value + 1 to read all elements including the element you are searching for.
        /// </returns>
        public int DistanceTo(T Value)
        {
            int Distance = 0;

            for (int i = _ReadPtr + _SeekOffset; i != _WritePtr; i = (i + 1) % _Buffer.Length)
            {
                if (Object.Equals(_Buffer[i], Value))
                    return Distance;

                Distance++;
            }

            return -1;
        }
    }
}
