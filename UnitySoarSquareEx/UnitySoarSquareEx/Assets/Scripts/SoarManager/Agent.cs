using System;
using System.Runtime.InteropServices;
using smlUnity;

namespace smlUnity {
    public enum smlPrintEventId{
        smlPRINT_EVENT_BAD = -1,
        smlEVENT_ECHO = 46,
        smlEVENT_FIRST_PRINT_EVENT = 46,
        smlEVENT_PRINT = 47,
        smlEVENT_LAST_PRINT_EVENT = 47
    }
    public enum smlRunStepSize {
        sml_PHASE = 0,
        sml_ELABORATION = 1,
        sml_DECIDE = 2,
        sml_UNTIL_OUTPUT = 3
    }

    public class Agent{
        private IntPtr _pAgent;
        private string _name;
        private Kernel _kernel;

        public Agent(IntPtr pAgent, string name, Kernel kernel) {
            _pAgent = pAgent;
            _name = name;
            _kernel = kernel;
        }

#region From DLL
        [DllImport("SoarUnityAPI")]
        private static extern int loadProductions(IntPtr pAgent, string path, bool echoResults);

        //##################### Manage IO ######################
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getInputLink(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getOutputLink(IntPtr pAgent);

        //##################### Manage WMEs ######################
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createStringWME(IntPtr pAgent, IntPtr pParent, string atribute, string value);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createIntWME(IntPtr pAgent, IntPtr pParent, string atribute, long value);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createFloatWME(IntPtr pAgent, IntPtr pParent, string atribute, double value);

        [DllImport("SoarUnityAPI")]
        private static extern void updateStringWME(IntPtr pAgent, IntPtr pWME, string value);

        [DllImport("SoarUnityAPI")]
	    private static extern void updateIntWME(IntPtr pAgent, IntPtr pWME, long value);

        [DllImport("SoarUnityAPI")]
	    private static extern void updateFloatWME(IntPtr pAgent, IntPtr pWME, double value);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createIdWME(IntPtr pAgent, IntPtr pParent, string atribute);

        [DllImport("SoarUnityAPI")]
        private static extern void setBlinkIfNoChange(IntPtr pAgent, bool state);

        [DllImport("SoarUnityAPI")]
        private static extern bool isBlinkIfNoChange(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr createSharedIdWME(IntPtr pAgent, IntPtr parent, string attribute, IntPtr pSharedValue);

        [DllImport("SoarUnityAPI")]
        private static extern bool destroyWME(IntPtr pAgent, IntPtr pWME);
        
        [DllImport("SoarUnityAPI")]
        private static extern IntPtr initSoar(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern void setOutputLinkChangeTracking(IntPtr pAgent, bool setting);

        [DllImport("SoarUnityAPI")]
        private static extern int getNumberOutputLinkChanges(IntPtr pAgent );

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getOutputLinkChange(IntPtr pAgent, int index);

        [DllImport("SoarUnityAPI")]
        private static extern bool isOutputLinkChangeAdd(IntPtr pAgent, int index);

        [DllImport("SoarUnityAPI")]
        private static extern int getNumberCommands(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern bool commands(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern IntPtr getCommand(IntPtr pAgent, int index);
        
        [DllImport("SoarUnityAPI")]
        private static extern void commit(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern bool isCommitRequired(IntPtr pAgent);

        //##################### Events ######################
        //###Print 
        #region Print
        //TODO: Show text on the screen instead of printing into the console for better visualization

        public delegate void PrintEventHandler(smlPrintEventId eventID, IntPtr pUserData, IntPtr pAgent, IntPtr pMessage);

        [DllImport("SoarUnityAPI")]
        private static extern int registerForPrintEvent(IntPtr pAgent, smlPrintEventId eventID, PrintEventHandler handler, IntPtr pUserData, bool ignoreOwnEchos, bool addToBack);

        [DllImport("SoarUnityAPI")]
        private static extern bool unregisterForPrintEvent(IntPtr pAgent, int callbackId);

        #endregion

        //##################### Run ######################
        [DllImport("SoarUnityAPI")]
        private static extern string runSelf(IntPtr pAgent, int numberSteps, smlRunStepSize stepSize);

        [DllImport("SoarUnityAPI")]
        private static extern void runSelfForever(IntPtr pAgent);

        [DllImport("SoarUnityAPI")]
        private static extern void runSelfTilOutput(IntPtr pAgent);

#endregion

        /// <summary>
        /// Returns this agent's name.
        /// </summary>
        public string GetAgentName() {
            return _name;
        }

        /// <summary>
        /// Returns a pointer to the kernel object that owns this Agent.
        /// </summary>
        public Kernel GetKernel() {
            return _kernel;
        }

        /// <summary>
        /// Load a set of productions from a file.
        /// The file must currently be on a filesystem that the kernel can
        /// access (i.e. can't send to a remote PC unless that PC can load
        /// this file).
        /// Ps: For me only worked passing the full path. Use Application.dataPath + "PATH_FROM_ASSETS".
        ///</summary>
        /// <param name="echoResults"> If true the results are also echoed through the smlEVENT_ECHO event, so they can appear in a debugger (or other listener)</param>
        /// <returns>True if finds file to load successfully.</returns>
        public int LoadProductions(string path, bool echoResults = true) {
            return loadProductions(_pAgent, path, echoResults);
        }

        //##################### Manage IO ######################
        ///<summary> 
        /// Returns the id object for the input link.
        /// The agent retains ownership of this object.
        /// </summary>
        public Identifier GetInputLink() {
            return new Identifier(getInputLink(_pAgent));
        }

        ///<summary>
        /// Returns the id object for the output link.
        /// The agent retains ownership of this object.
        /// Note this will be null until the first time an agent
        /// puts something on the output link.
        ///</summary>
        public Identifier GetOutputLink() {
            return new Identifier(getOutputLink(_pAgent));
        }

        //##################### Manage WMEs ######################
        ///<summary>
        /// Builds a new WME that has a string value and schedules
        /// it for addition to Soar's input link.
        ///
        /// The agent retains ownership of this object.
        ///
        /// The returned object is valid until the caller
        /// deletes the parent identifier.
        ///
        ///
        /// If "auto commit" is turned off in ClientKernel,
        /// the WME is not added to Soar's input link until the
        /// client calls "Commit".
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public StringElement CreateStringWME(Identifier pParent, string atribute, string value) {
            return new StringElement(createStringWME(_pAgent, pParent.GetPtr(), atribute, value));
        }

        ///<summary>
        /// Same as CreateStringWME but for a new WME that has an int as its value.     
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public IntPtr CreateIntWME(Identifier pParent, string atribute, long value) {
            return createIntWME(_pAgent , pParent.GetPtr(), atribute, value);
        }
        

        ///<summary>
        /// Same as CreateStringWME but for a new WME that has an floating point 
        /// as its value.     
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public IntPtr CreateFloatWME(Identifier pParent, string atribute, double value) {
            return createFloatWME(_pAgent, pParent.GetPtr(), atribute, value);
        }

        ///<summary>
        /// Same as CreateStringWME but for a new WME that has
        /// an identifier as its value.
        /// The identifier value is generated here and will be
        /// different from the value Soar assigns in the kernel.
        /// The kernel will keep a map for translating back and forth.
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public Identifier CreateIdWME(Identifier pParent, string atribute) {
            return new Identifier(createIdWME(_pAgent, pParent.GetPtr(), atribute));
        }

        ///<summary>
        /// Creates a new WME that has an identifier as its value.
        /// The value in this case is the same as an existing identifier.
        /// This allows us to create a graph rather than a tree.
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public Identifier CreateSharedIdWME(Identifier parent, string attribute, IntPtr pSharedValue) {
            return new Identifier(createSharedIdWME(_pAgent, parent.GetPtr(), attribute, pSharedValue));
        }

        ///<summary>
        /// Update the value of an existing WME.
        /// If "auto commit" is turned off in ClientKernel,
        /// the value is not actually sent to the kernel
        /// until "Commit" is called.
        ///
        /// If "BlinkIfNoChange" is false then updating a wme to the
        /// same value it already had will be ignored.
        /// This value is true by default, so updating a wme to the same
        /// value causes the wme to be deleted and a new identical one to be added
        /// which will trigger rules to rematch.
        /// You can turn this flag on and off around a set of calls to update if you wish.
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public void Update(StringElement pWME, string value) {
            updateStringWME(_pAgent, pWME.GetPtr(), value);
        }
	    public void Update(IntElement pWME, long value) {
            updateIntWME(_pAgent, pWME.GetPtr(), value);
        }
	    public void Update(FloatElement pWME, double value) {
            updateFloatWME(_pAgent, pWME.GetPtr(), value);
        }

        ///<summary>
        /// This flag controls whether updating a wme to the same
        /// value that it already has causes it to "blink" or not.
        /// Blinking means the wme is removed and an identical wme is added,
        /// causing rules that test this wme to be rematched and to fire again.
        ///</summary>
        public void SetBlinkIfNoChange(bool state) {
            setBlinkIfNoChange(_pAgent, state);
        }

        public  bool isBlinkIfNoChange() {
            return isBlinkIfNoChange(_pAgent);
        }

        ///<summary>
        /// Schedules a WME from deletion from the input link and removes
        /// it from the client's model of working memory.
        /// If this is an identifier then all of its children will be
        /// deleted too (assuming it's the only parent -- i.e. part of a tree not a full graph).
        /// The caller should not access this WME after calling
        /// DestroyWME() or any of its children if this is an identifier.
        /// If "auto commit" is turned off in ClientKernel,
        /// the WME is not removed from the input link until
        /// the client calls "Commit"
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        public bool DestroyWME(IntPtr pWME) {
            return destroyWME(_pAgent, pWME);
        }

        ///<summary>
        /// Reinitialize this Soar agent.
        /// This will also cause the output link structures stored
        /// here to be erased and the current input link to be sent over
        /// to the Soar agent for the start of its next run.
        /// Ps: https://www.mono-project.com/docs/advanced/pinvoke/#strings-as-return-values
        ///</summary>
        public string InitSoar() {
            return Marshal.PtrToStringAnsi(initSoar(_pAgent));
        }

        ///<summary>
        /// Enable or disable output-link change tracking. Do
        /// NOT use if using Commands, GetCommand,
        /// GetOutputLinkChange, AddOutputHandler.
        ///</summary>
        public void SetOutputLinkChangeTracking(bool setting) {
            setOutputLinkChangeTracking(_pAgent, setting);
        }

        ///<summary> Get number of changes to output link since last cycle.</summary>
        public int GetNumberOutputLinkChanges() {
            return getNumberOutputLinkChanges(_pAgent);
        }

        ///<summary> Get the n-th wme added or deleted to output link since last cycle.</summary>
        public IntPtr GetOutputLinkChange(int index) {
            return getOutputLinkChange(_pAgent, index);
        }

        ///<summary>
        /// Returns true if the n-th wme change to the output-link
        /// since the last cycle was a wme being added.
        /// (false => it was a wme being deleted)
        ///</summary>
        public bool IsOutputLinkChangeAdd(int index) {
            return isOutputLinkChangeAdd(_pAgent, index);
        }

        ///<summary>
        /// Get the number of "commands".  A command in this context
        /// is an identifier wme that have been added to the top level of
        /// the output-link since the last decision cycle.
        ///
        /// NOTE: This function may involve searching a list so it's
        /// best to not call it repeatedly.
        ///</summary>
        public int GetNumberCommands() {
            return getNumberCommands(_pAgent);
        }

        ///<summary>
        /// Returns true if there are "commands" available.
        /// A command in this context is an identifier wme that
        /// has been added to the top level of the output-link
        /// since the last decision cycle.
        ///
        /// NOTE: This function may involve searching a list so it's
        /// best to not call it repeatedly.
        ///</summary>
        public bool Commands() {
            return commands(_pAgent);
        }

        ///<summary>
        /// Get the n-th "command".  A command in this context
        /// is an identifier wme that have been added to the top level of
        /// the output-link since the last decision cycle.
        ///
        /// Special note about output-link WMEs: The agent is
        /// free to remove WMEs from the output-link at any time.
        /// If you retain a WME for multiple decision cycles,
        /// you must check output link changes (using
        /// GetNumOutputLinkChanges, GetOutputLinkChange, and
        /// IsOutputLinkAdd) to check if the WMEs you have were
        /// removed during the last decision cycle. Dereferencing
        /// a removed WME causes a segmentation fault.
        ///</summary>
        ///
        /// <param name="index"> The 0-based index for which command to get.</param>
        /// <returns>Returns NULL if index is out of range.</returns>
        public Identifier GetCommand(int index) {
            return new Identifier(getCommand(_pAgent, index));
        }

        ///<summary>
        /// Send the most recent list of changes to working memory
        /// over to the kernel.
        ///</summary>
        public void Commit() {
            commit(_pAgent);
        }

        ///<summary> Returns true if this agent has uncommitted changes. </summary>
        public bool IsCommitRequired() {
            return isCommitRequired(_pAgent);
        }

        //##################### Events ######################
        //TODO: Output Handler

        //TODO: Run Event (not working, like dont work even in pure C++)

        //TODO: Output Notification

        //TODO: Production Event

        ///<summary> 
        /// Register for an "PrintEvent".
        /// Multiple handlers can be registered for the same event.
        ///
        /// Current set is: smlEVENT_PRINT
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
        ///<param name="smlEventId">     The event we're interested in (see the list below for valid values)</param>
        ///<param name="handler">        A function that will be called when the event happens</param>
        ///<param name="pUserData">      Arbitrary data that will be passed back to the handler function when the event happens.</param>
        ///<param name="ignoreOwnEchos"> If true and registering for echo event, commands issued through this connection won't echo.  If false, echos all commands.</param>  Ignored for non-echo events.
        ///<param name="addToBack">      If true add this handler is called after existing handlers.  If false, called before existing handlers.</param>
        ///
        ///<returns> A unique ID for this callback (used to unregister the callback later)</returns>
        public int RegisterForPrintEvent(smlPrintEventId eventID, PrintEventHandler handler, IntPtr pUserData, bool ignoreOwnEchos = true, bool addToBack = true) {
            return registerForPrintEvent(_pAgent, eventID, handler, pUserData, ignoreOwnEchos, addToBack);
        }

        ///<summary> Unregister for a particular event </summary>
        public  bool UnregisterForPrintEvent(int callbackId) {
            return unregisterForPrintEvent(_pAgent, callbackId);
        }

        //TODO: XML Event

        //##################### Run ######################
        ///<summary> Run one Soar agent for the specified number of decisions </summary>
        ///
        ///<retunrs>
        /// The result of executing the run command.
        /// The output from during the run is sent to a different callback.
        ///</retunrs>
        public string RunSelf(int numberSteps, smlRunStepSize stepSize = smlRunStepSize.sml_DECIDE) {
            return runSelf(_pAgent, numberSteps, stepSize);
        }
        public void RunSelfForever() {
            runSelfForever(_pAgent);
        }

        ///<summary>
        /// Run Soar until either output is generated or
        /// the maximum number of decisions is reached.
        ///
        /// We don't generally want Soar to just run until it generates
        /// output without any limit as an error in the AI logic might cause
        /// it to never return control to the environment, so there is a maximum
        /// decision count (currently 15) and if the agent fails to produce output
        /// before then this command returns.  (This value can be changed with the
        /// max-nil-output-cycles command).
        ///</summary>
        public void RunSelfTilOutput() {
            runSelfTilOutput(_pAgent);
        }

        //TODO: Functions bellow RunSelfTilOutput
    }

}