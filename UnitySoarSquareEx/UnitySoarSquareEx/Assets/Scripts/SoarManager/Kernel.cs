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
        public const int kDefaultSMLPort = 12121;
        public const int kSuppressListener = 0;
        public const int kUseAnyPort = -1;

        private IntPtr _pKernel;

        public Kernel(IntPtr pKernel){
            _pKernel = pKernel;
        }

#region  From DLL
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createKernelInNewThread(int portToListenOn);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createKernelInCurrentThread(bool optimized, int portToListenOn);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createAgent(string name, IntPtr pKernel);

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
        ///<summary>
        /// Creates a connection to the Soar kernel that is embedded
        ///  within the same process as the caller.
        ///
        /// If you're not sure which method to use, you generally want "InNewThread".
        /// That extra thread usually makes your life easier.
        ///
        /// Creating in "current thread" will produce maximum performance but requires a little more work for the developer
        /// (you must call CheckForIncomingCommands() periodically and you should not register for events and then go to sleep).
        ///
        /// Creating in "new thread" is simpler for the developer but will be slower (around a factor 2).
        /// (It's simpler because there's no need to call CheckForIncomingCommands() periodically as this happens in a separate
        /// thread running inside the kernel and incoming events are handled by another thread in the client).
        ///</summary>
        ///<param name="Optimized"> 
        /// If this is a current thread connection, we can short-circuit parts of the messaging system for sending input and
        /// running Soar.  If this flag is true we use those short cuts.  If you're trying to debug the SML libraries
        /// you may wish to disable this option (so everything goes through the standard paths).  Not available if running in a new thread.
        /// Also if you're looking for maximum performance be sure to read about the "auto commit" options below.
        ///</param>
        ///
        /// <param name="port">   
        /// The port number the kernel should use to receive remote connections.  The default port for SML is 12121 (kDefaultSMLPort)
        /// (picked at random). Passing 0 (kSuppressListener) means no listening port will be created (so it will be impossible to make
        /// remote connections to the kernel). Passing -1 (kUseAnyPort) means bind to any availble port (retrieve after success using
        /// GetListenerPort()), and, for local connections, create a named pipe using the PID. To connect to this high-performance
        /// local connection, simply pass the PID as the port in CreateRemoteConnection().
        ///</param>
        ///
        ///<returns>
        /// A new kernel object which is used to communicate with the kernel.
        /// If an error occurs a Kernel object is still returned.  Call "HadError()" and "GetLastErrorDescription()" on it.
        ///</returns>
        public static Kernel CreateKernelInNewThread(int portToListenOn = kDefaultSMLPort) {
            IntPtr pKernel = createKernelInNewThread(portToListenOn);
            return new Kernel(pKernel);
        }

        Kernel CreateKernelInCurrentThread(bool optimized = false, int portToListenOn = kDefaultSMLPort) {
            IntPtr pKernel = createKernelInCurrentThread(optimized, portToListenOn);
            return new Kernel(pKernel);
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
        /// <summary> 
        /// Creates a new Soar agent with the given name.
        /// This object is owned by the kernel and will be destroyed 
        /// when the kernel is destroyed.
        ///</summary>
        ///
        ///<returns> A pointer to the agent (or NULL if not found).</returns>  
        public Agent CreateAgent(string name) {
            IntPtr pAgent = createAgent(name, _pKernel);
            return new Agent(pAgent, name, this);
        }

        //#### Update
        ///<summary>
        /// Register for an "UpdateEvent".
        /// Multiple handlers can be registered for the same event.
        /// This event is registered with the kernel because they relate to events we think may be useful to use to trigger updates
        /// in synchronous environments.
        ///
        /// Ps: To send a object as a IntPtr in the userData parameters use:
        /// GCHandle data = GCHandle.Alloc(YOUR_OBJECT);
        /// IntPtr dataPtr = GCHandle.ToIntPtr(data);
        /// A pointer allocated in that way can than be type casted like:
        /// YOUR_OBJECT myObj = (YOUR_OBJECT_TYPE)((GCHandle)userDataPtr).Target;
        /// And to free it after use:
        /// data.Free();
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