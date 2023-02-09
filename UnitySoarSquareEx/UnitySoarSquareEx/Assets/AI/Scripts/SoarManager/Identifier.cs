using System;
using System.Runtime.InteropServices;

namespace smlUnity {
    public class Identifier {
        private static IntPtr _pIdentifier;

        public Identifier(IntPtr pIdentifier) {
            _pIdentifier = pIdentifier;
        }

#region From DLL
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getCommandName(IntPtr pIdentifier);

        [DllImport("SoarUnityAPI")]
        private static extern void addStatusComplete(IntPtr pIdentifier);

        [DllImport("SoarUnityAPI")]
        private static extern void addStatusError(IntPtr pIdentifier);
#endregion

        public IntPtr GetPtr(){
            return _pIdentifier;
        }

        ///<summary>
        /// Returns the "command name" for a top-level identifier on the output-link.
        /// That is for output-link O1 (O1 ^move M3) returns "move".
        /// Ps: https://www.mono-project.com/docs/advanced/pinvoke/#strings-as-return-values
        ///</summary>
        public string GetCommandName() {
            return Marshal.PtrToStringAnsi(getCommandName(_pIdentifier));
        }

         ///<summary>Adds "^status complete" as a child of this identifier.</summary>
        public void AddStatusComplete(){
            addStatusComplete(_pIdentifier);
        }

        ///<summary>Adds "^status error" as a child of this identifier.</summary>
        public void AddStatusError(){
            addStatusError(_pIdentifier);
        }
    }
}