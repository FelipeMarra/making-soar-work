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

        private IntPtr _pKernel;

        public Kernel(){
            _pKernel = createKernelInNewThread();
        }

#region  From DLL
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createKernelInNewThread();

        [DllImport("SoarUnityAPI")]
        private static extern void shutdown(IntPtr Kernel);

        //##################### Manage WMEs ######################
        [DllImport("SoarUnityAPI")]
        private static extern void setAutoCommit(IntPtr Kernel, bool state);
        
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

        /// <summary>
        /// Returns this kernel's pointer.
        /// </summary>
        public IntPtr GetPtr(){
            return _pKernel;
        }

        ///<summary>
        /// Preparation for deleting the kernel.
        /// Agents are destroyed at this point (if we own the kernel)
        /// After calling shutdown the kernel cannot be restarted
        /// it must be deleted.
        /// This is separated from delete to ensure that messages
        /// relating to system shutdown can be sent in a more stable
        /// state (while the kernel object still exists).
        ///</summary>
        public void Shutdown(){
            shutdown(_pKernel);
        }

        //##################### Manage WMEs ######################
        ///<summary>
        /// If auto commit is set to false then after making any changes
        /// to working memory elements (adds, updates, deletes) the client
        /// must call "commit" to send those changes over to Soar.
        /// This mode boosts performance because only a single packet
        /// is sent containing all of the wme changes.
        ///
        /// If auto commit is set to true then after any change to
        /// working memory, commit is called automatically and the
        /// change is sent immediately over to the kernel.
        /// This makes the developer's life easier (no need to remember
        /// to call commit) at the cost of some performance.
        ///</summary>
        public void SetAutoCommit(bool state){
            setAutoCommit(_pKernel, state);
        }

        //##################### Events ######################
        //#### Update
        ///<summary>
        /// Register for an "UpdateEvent".
        /// Multiple handlers can be registered for the same event.
        /// This event is registered with the kernel because they relate to events we think may be useful to use to trigger updates
        /// in synchronous environments.
        ///
        /// Ps: To send a object as a IntPtr in the userData parameters use:
        /// GCHandle data = GCHandle.Alloc(YOUR_OBJECT);
        /// IntPtr dataPtr = GCHandle.ToIntPtr(userData);
        /// A pointer allocated in that way can than be type casted like:
        /// YOUR_OBJECT data = (YOUR_OBJECT_TYPE)((GCHandle)userDataPtr).Target;
        /// And don't forget to free it afeter use: data.Free()
        ///</summary>
        ///
        ///<param name="smlEventId">The event we're interested in (see the list below for valid values)</param>
        ///<param name="handler">A function that will be called when the event happens</param>
        ///<param name="pUserData">Arbitrary data that will be passed back to the handler function when the event happens.</param>
        ///<param name="addToBack">If true add this handler is called after existing handlers.  If false, called before existing handlers.</param>
        ///
        ///<returns>A unique ID for this callback (used to unregister the callback later)</returns>
        public int RegisterForUpdateEvent(smlUpdateEventId id, UpdateEventHandler handler, IntPtr pUserData, bool addToBack = true){
            return registerForUpdateEvent(_pKernel, id, handler, pUserData, addToBack);
        }

        ///<summary>Unregister for a particular event</summary>
        ///<returns>True if succeeds</returns>
        public bool UnregisterForUpdateEvent(int callbackId){
            return unregisterForUpdateEvent(_pKernel, callbackId);
        }
    }
}