using System;
using System.Runtime.InteropServices;

namespace smlUnity{
    public enum smlUpdateEventId {
        smlUPDATE_EVENT_BAD = -1,
        smlEVENT_AFTER_ALL_OUTPUT_PHASES = 53,
        smlEVENT_AFTER_ALL_GENERATED_OUTPUT = 54,
        smlEVENT_LAST_UPDATE_EVENT = 54
    }
    public enum smlRunFlags {
        sml_NONE = 0,
        sml_RUN_SELF = 1,
        sml_RUN_ALL = 2,
        sml_UPDATE_WORLD = 4,
        sml_DONT_UPDATE_WORLD = 8
    }

    public class Kernel {

        IntPtr _pKernel;

        public Kernel(){
            _pKernel = createKernelInNewThread();
        }

#region  From DLL
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createKernelInNewThread();

        //##################### Manage WMEs ######################
        [DllImport("SoarUnityAPI")]
        private static extern  void setAutoCommit(IntPtr Kernel, bool state);
        
        //##################### Events ######################
        //#### Update
        #region Update

        public delegate void UpdateEventHandler(smlUpdateEventId eventID, IntPtr pUserData, IntPtr pKernel, smlRunFlags runFlags);

        [DllImport("SoarUnityAPI")]
        private static extern int registerForUpdateEvent(IntPtr pKernel, smlUpdateEventId id, UpdateEventHandler handler, IntPtr pUserData, bool addToBack);

        [DllImport("SoarUnityAPI")]
        private static extern bool unregisterForUpdateEvent(IntPtr pKernel, int callbackId);

        #endregion
#endregion

        public IntPtr GetPtr(){
            return _pKernel;
        }

        //##################### Manage WMEs ######################
        public void SetAutoCommit(bool state){
            setAutoCommit(_pKernel, state);
        }

        //##################### Events ######################
        //#### Update
        public int RegisterForUpdateEvent(smlUpdateEventId id, UpdateEventHandler handler, IntPtr pUserData, bool addToBack = true){
            return registerForUpdateEvent(_pKernel, id, handler, pUserData, addToBack);
        }

        public bool UnregisterForUpdateEvent(int callbackId){
            return unregisterForUpdateEvent(_pKernel, callbackId);
        }
    }
}