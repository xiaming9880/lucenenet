using Lucene.Net.Diagnostics;
using System;
using System.Runtime.CompilerServices;

namespace Lucene.Net.Util.Fst
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    using DataInput = Lucene.Net.Store.DataInput;
    using DataOutput = Lucene.Net.Store.DataOutput;

    /// <summary>
    /// An FST <see cref="Outputs{T}"/> implementation where each output
    /// is a non-negative <see cref="T:long?"/> value.
    /// <para/>
    /// NOTE: This was PositiveIntOutputs in Lucene
    /// <para/>
    /// @lucene.experimental
    /// </summary>
    public sealed class PositiveInt32Outputs : Outputs<long?>
    {
        private const long NO_OUTPUT = new long();

        private static readonly PositiveInt32Outputs singleton = new PositiveInt32Outputs();

        private PositiveInt32Outputs()
        {
        }

        public static PositiveInt32Outputs Singleton => singleton;

        public override long? Common(long? output1, long? output2)
        {
            if (Debugging.AssertsEnabled)
            {
                Debugging.Assert(Valid(output1));
                Debugging.Assert(Valid(output2));
            }
            if (output1 == NO_OUTPUT || output2 == NO_OUTPUT)
            {
                return NO_OUTPUT;
            }
            else
            {
                if (Debugging.AssertsEnabled)
                {
                    Debugging.Assert(output1 > 0);
                    Debugging.Assert(output2 > 0);
                }
                return Math.Min(output1.Value, output2.Value);
            }
        }

        public override long? Subtract(long? output, long? inc)
        {
            if (Debugging.AssertsEnabled)
            {
                Debugging.Assert(Valid(output));
                Debugging.Assert(Valid(inc));
                Debugging.Assert(output >= inc);
            }

            if (inc == NO_OUTPUT)
            {
                return output;
            }
            else if (output.Equals(inc))
            {
                return NO_OUTPUT;
            }
            else
            {
                return output - inc;
            }
        }

        public override long? Add(long? prefix, long? output)
        {
            if (Debugging.AssertsEnabled)
            {
                Debugging.Assert(Valid(prefix));
                Debugging.Assert(Valid(output));
            }
            if (prefix == NO_OUTPUT)
            {
                return output;
            }
            else if (output == NO_OUTPUT)
            {
                return prefix;
            }
            else
            {
                return prefix + output;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(long? output, DataOutput @out)
        {
            if (Debugging.AssertsEnabled) Debugging.Assert(Valid(output));
            @out.WriteVInt64(output.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long? Read(DataInput @in)
        {
            long v = @in.ReadVInt64();
            if (v == 0)
            {
                return NO_OUTPUT;
            }
            else
            {
                return v;
            }
        }

        private bool Valid(long? o)
        {
            Debugging.Assert(o != null, "PositiveIntOutput precondition fail");
            Debugging.Assert(o == NO_OUTPUT || o > 0,"o={0}", o);
            return true;
        }

        public override long? NoOutput => NO_OUTPUT;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string OutputToString(long? output)
        {
            return output.ToString(); // LUCENENET TODO: Invariant Culture?
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return "PositiveIntOutputs";
        }
    }
}