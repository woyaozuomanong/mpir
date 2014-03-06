﻿/*
Copyright 2014 Alex Dyachenko

This file is part of the MPIR Library.

The MPIR Library is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published
by the Free Software Foundation; either version 3 of the License, or (at
your option) any later version.

The MPIR Library is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
License for more details.

You should have received a copy of the GNU Lesser General Public License
along with the MPIR Library.  If not, see http://www.gnu.org/licenses/.  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniGruntWare.Magic.Wand;

namespace Spells
{
    /// <summary>
    /// This spell generates a typical set of unit tests and adds it to the active file.
    /// </summary>
    internal class AddTest : ISpell
    {
        public string Name
        {
            get { return "Add &Integer Test"; }
        }

        public void Execute(IDevelopmentEnvironment env)
        {
            var args = env.Prompt("Add Test")
                .Instructions(string.Format("Add a set of unit tests for an MPIR HugeInt method family to the active file ({0}):", env.ActiveFile.Name))
                .ForString("Method").Label("&Method under test:").PreviousDefaultOr("").Required()
                .ForString("InitialValue").Label("&Initial value of the class:").PreviousDefaultOr("").Required()
                .ForBoolean("FromNone").Label("Test with&out arguments:").PreviousDefaultOr(false)
                .ForString("NoArgumentsResultValue").Label("Res&ult Value:").PreviousDefaultOr("").Required().EnabledBy("FromNone")
                .ForBoolean("FromHugeInt").Label("Test with another &HugeInt:").PreviousDefaultOr(true)
                .ForString("HugeIntInitialValue").Label("&With Initial Value:").PreviousDefaultOr("").Required().EnabledBy("FromHugeInt")
                .ForString("HugeIntResultValue").Label("&Result Value:").PreviousDefaultOr("").Required().EnabledBy("FromHugeInt")
                .ForBoolean("FromLimb").Label("Test with a &Limb:").PreviousDefaultOr(true)
                .ForString("LimbInitialValue").Label("Wi&th Initial Value:").PreviousDefaultOr("").Required().EnabledBy("FromLimb")
                .ForString("LimbResultValue").Label("R&esult Value:").PreviousDefaultOr("").Required().EnabledBy("FromLimb")
                .ForBoolean("FromSignedLimb").Label("Test with a signed Lim&b:").PreviousDefaultOr(true)
                .ForString("SignedLimbInitialValue").Label("With Initi&al Value:").PreviousDefaultOr("").Required().EnabledBy("FromSignedLimb")
                .ForString("SignedLimbResultValue").Label("Re&sult Value:").PreviousDefaultOr("").Required().EnabledBy("FromSignedLimb")
                .Run();

            //args will be null if the user cancels the prompt
            if (args == null)
                return;

            var methods = new List<string>();

            if ((bool)args["FromNone"])
            {
                methods.Add(Format(@"
                    [TestMethod]
                    public void {0}()
                    {{
                        using (var a = new HugeInt(""{1}""))
                        {{
                            a.{0}();
                            Assert.AreEqual(""{2}"", a.ToString());
                        }}
                    }}",
                    args["Method"],
                    args["InitialValue"],
                    args["NoArgumentsResultValue"]));
            }

            if ((bool)args["FromHugeInt"])
            {
                methods.Add(Format(@"
                    [TestMethod]
                    public void {0}HugeInt()
                    {{
                        using (var a = new HugeInt(""{1}""))
                        using (var b = new HugeInt(""{2}""))
                        {{
                            a.{0}(b);
                            Assert.AreEqual(""{3}"", a.ToString());
                        }}
                    }}",
                    args["Method"],
                    args["InitialValue"],
                    args["HugeIntInitialValue"],
                    args["HugeIntResultValue"]));
            }

            if ((bool)args["FromLimb"])
            {
                methods.Add(Format(@"
                    [TestMethod]
                    public void {0}Limb()
                    {{
                        using (var a = new HugeInt(""{1}""))
                        {{
                            ulong b = {2};
                            a.{0}(b);
                            Assert.AreEqual(""{3}"", a.ToString());
                        }}
                    }}",
                    args["Method"],
                    args["InitialValue"],
                    args["LimbInitialValue"],
                    args["LimbResultValue"]));
            }

            if ((bool)args["FromSignedLimb"])
            {
                methods.Add(Format(@"
                    [TestMethod]
                    public void {0}SignedLimb()
                    {{
                        using (var a = new HugeInt(""{1}""))
                        {{
                            long b = {2};
                            a.{0}(b);
                            Assert.AreEqual(""{3}"", a.ToString());
                        }}
                    }}",
                    args["Method"],
                    args["InitialValue"],
                    args["SignedLimbInitialValue"],
                    args["SignedLimbResultValue"]));
            }

            env.ActiveFile.Before("//more tests coming here")
                .InsertMembers(methods.ToArray());
        }

        private string Format(string format, params object[] args)
        {
            return string.Format(format, args)
                .Replace("\r", "")
                .Replace("\n            ", Environment.NewLine);
        }
    }
}
