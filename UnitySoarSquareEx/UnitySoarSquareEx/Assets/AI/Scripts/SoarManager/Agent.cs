using System;
using System.Runtime.InteropServices;

namespace smlUnity {
    public enum smlPrintEventId{
        smlPRINT_EVENT_BAD = -1,
        smlEVENT_ECHO = 46,
        smlEVENT_FIRST_PRINT_EVENT = 46,
        smlEVENT_PRINT = 47,
        smlEVENT_LAST_PRINT_EVENT = 47
    }

    public class Agent{
        private static IntPtr _pAgent;

        public Agent(string name, Kernel kernel){
            _pAgent = createAgent(name, kernel.GetPtr());
        }

#region From DLL
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createAgent(string name, IntPtr pKernel);

        [DllImport("SoarUnityAPI")]
        private static extern int loadProductions(IntPtr pAgent, string path);

        //##################### Manage IO ######################
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getInputLink(IntPtr pAgent);

        //##################### Manage WMEs ######################
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createIdWME(IntPtr pAgent, IntPtr pParent, string atribute);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createStringWME(IntPtr pAgent, IntPtr pParent, string atribute, string value);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createIntWME(IntPtr pAgent, IntPtr pParent, string atribute, long value);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createFloatWME(IntPtr pAgent, IntPtr pParent, string atribute, double value);

        [DllImport("SoarUnityAPI")]
        private static extern void commit(IntPtr pAgent);

        //##################### Run ######################
        [DllImport("SoarUnityAPI")]
        private static extern void runSelfTilOutput(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern void runSelfForever(IntPtr pAgent);

        //##################### Events ######################
        //###Print 
        #region Print
        //TODO: Show text on the screen instead of printing into the console for better visualization

        public delegate void PrintEventHandler(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, string message);

        [DllImport("SoarUnityAPI")]
        private static extern int registerForPrintEvent(IntPtr pAgent, smlPrintEventId eventID, PrintEventHandler handler, IntPtr pUserData, bool ignoreOwnEchos, bool addToBack);

        [DllImport("SoarUnityAPI")]
        private static extern bool unregisterForPrintEvent(IntPtr pAgent, int callbackId);

        #endregion

#endregion

        public int LoadProductions(string path) {
            return loadProductions(_pAgent, path);
        }

        //##################### Manage IO ######################
        public IntPtr GetInputLink(){
            return getInputLink(_pAgent);
        }

        //##################### Manage WMEs ######################
        public IntPtr CreateIdWME(IntPtr pParent, string atribute){
            return createIdWME(_pAgent, pParent, atribute);
        }

        public IntPtr CreateStringWME(IntPtr pParent, string atribute, string value) {
            return createStringWME(_pAgent, pParent, atribute, value);
        }

        public IntPtr CreateIntWME(IntPtr pParent, string atribute, long value) {
            return createIntWME(_pAgent , pParent, atribute, value);
        }
        

        public IntPtr CreateFloatWME(IntPtr pParent, string atribute, double value) {
            return createFloatWME(_pAgent, pParent, atribute, value);
        }


        public void Commit() {
            commit(_pAgent);
        }

        //##################### Run ######################
        public void RunSelfTilOutput() {
            runSelfTilOutput(_pAgent);
        }

        public void RunSelfForever() {
            runSelfForever(_pAgent);
        }

        //##################### Events ######################
        public int RegisterForPrintEvent(smlPrintEventId eventID, PrintEventHandler handler, IntPtr pUserData, bool ignoreOwnEchos = true, bool addToBack = true) {
            return registerForPrintEvent(_pAgent, eventID, handler, pUserData, ignoreOwnEchos, addToBack);
        }

        public  bool UnregisterForPrintEvent(int callbackId){
            return unregisterForPrintEvent(_pAgent, callbackId);
        }

        //##################### Events ######################
        //TODO not working (dont work even in pure C++)
        // [DllImport("SoarUnityAPI")]
        // public static extern int registerForProductionAddedEvent(IntPtr agent);
    }

}