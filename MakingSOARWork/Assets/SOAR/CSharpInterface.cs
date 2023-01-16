using System;
using System.Runtime.InteropServices;
using sml;
using UnityEngine;

namespace TestCSharpSML {
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class CSharpInterface {
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			CSharpInterface test = new CSharpInterface();

			bool result = false;
			try {
				result = test.Test();
			}
			catch (Exception ex) {
				Debug.Log(ex);
			}

			Debug.Log("-----------------------------");
			if (result)
				Debug.Log("Tests assed");
			else
				Debug.Log("Tests failed");
		}

		bool Test() {
			Kernel kernel = Kernel.CreateKernelInNewThread();

			// Make sure the kernel was ok
			if (kernel.HadError())
				throw new Exception("Error initializing kernel: " + kernel.GetLastErrorDescription());

			Agent agent = kernel.CreateAgent("First");

			// We test the kernel for an error after creating an agent as the agent
			// object may not be properly constructed if the create call failed so
			// we store errors in the kernel in this case.  Once this create is done we can work directly with the agent.
			if (kernel.HadError())
				throw new Exception("Error creating agent: " + kernel.GetLastErrorDescription());

			bool ok = agent.LoadProductions(Application.dataPath + "\\SOAR\\test_soar.soar");
			if (!ok) {
				Debug.Log("Load failed"); 
				return false;
			}

			Identifier inputLink = agent.GetInputLink();

			if (inputLink == null)
				throw new Exception("Error getting the input link");

            inputLink.CreateStringWME("hello", "world");
			//ok = agent.Commit();

			// Quick test of init-soar
			agent.InitSoar();

			String myTestData = "my data";
			Agent.RunEventCallback runCall = new Agent.RunEventCallback(MyRunEventCallback);			
			Agent.ProductionEventCallback prodCall = new Agent.ProductionEventCallback(MyProductionEventCallback);			
			Agent.PrintEventCallback printCall = new Agent.PrintEventCallback(MyPrintEventCallback);			
			Agent.OutputEventCallback outputCall = new Agent.OutputEventCallback(MyOutputEventCallback);
			Agent.XMLEventCallback xmlCall		 = new Agent.XMLEventCallback(MyXMLEventCallback);
			Agent.OutputNotificationCallback noteCall = new Agent.OutputNotificationCallback(MyOutputNotificationCallback);
			Kernel.AgentEventCallback agentCall = new Kernel.AgentEventCallback(MyAgentEventCallback);
			Kernel.SystemEventCallback systemCall = new Kernel.SystemEventCallback(MySystemEventCallback);	
			Kernel.UpdateEventCallback updateCall = new Kernel.UpdateEventCallback(MyUpdateEventCallback);
			Kernel.StringEventCallback strCall    = new Kernel.StringEventCallback(MyStringEventCallback);
			Kernel.RhsFunction rhsCall			  = new Kernel.RhsFunction(MyTestRhsFunction);
			Kernel.ClientMessageCallback clientCall	= new Kernel.ClientMessageCallback(MyTestClientMessageCallback);

			int runCallbackID	= agent.RegisterForRunEvent(smlRunEventId.smlEVENT_AFTER_DECISION_CYCLE, runCall, myTestData);
			int prodCallbackID	= agent.RegisterForProductionEvent(smlProductionEventId.smlEVENT_AFTER_PRODUCTION_FIRED, prodCall, myTestData);
			int printCallbackID	= agent.RegisterForPrintEvent(smlPrintEventId.smlEVENT_PRINT, printCall, myTestData);
			int outputCallbackID= agent.AddOutputHandler("move", outputCall, myTestData);
			int noteCallbackID  = agent.RegisterForOutputNotification(noteCall, myTestData);
			int xmlCallbackID   = agent.RegisterForXMLEvent(smlXMLEventId.smlEVENT_XML_TRACE_OUTPUT, xmlCall, myTestData);
			int agentCallbackID	= kernel.RegisterForAgentEvent(smlAgentEventId.smlEVENT_BEFORE_AGENT_REINITIALIZED, agentCall, myTestData);
            int systemCallbackID = kernel.RegisterForSystemEvent(smlSystemEventId.smlEVENT_SYSTEM_START, systemCall, myTestData);
			int updateCallbackID= kernel.RegisterForUpdateEvent(smlUpdateEventId.smlEVENT_AFTER_ALL_OUTPUT_PHASES, updateCall, myTestData);
			//int stringCallbackID= kernel.RegisterForStringEvent(smlStringEventId.smlEVENT_EDIT_PRODUCTION, strCall, myTestData);
			int rhsCallbackID	= kernel.AddRhsFunction("test-rhs", rhsCall, myTestData);
			int clientCallbackID= kernel.RegisterForClientMessageEvent("test-client", clientCall, myTestData);

			// Running the agent will trigger most of the events we're listening for so
			// we can check that they're working correctly.
			//agent.RunSelf(3);
			kernel.RunAllAgents(5);

			// Trigger an agent event
			agent.InitSoar();

			ok = agent.UnregisterForRunEvent(runCallbackID);
			ok = ok && agent.UnregisterForProductionEvent(prodCallbackID);
			ok = ok && agent.UnregisterForPrintEvent(printCallbackID);
			ok = ok && agent.RemoveOutputHandler(outputCallbackID);
			ok = ok && agent.UnregisterForOutputNotification(noteCallbackID);
			ok = ok && agent.UnregisterForXMLEvent(xmlCallbackID);
			ok = ok && kernel.UnregisterForAgentEvent(agentCallbackID);
			ok = ok && kernel.UnregisterForSystemEvent(systemCallbackID);
			ok = ok && kernel.UnregisterForUpdateEvent(updateCallbackID);
			//ok = ok && kernel.UnregisterForStringEvent(stringCallbackID);
			ok = ok && kernel.RemoveRhsFunction(rhsCallbackID);
			ok = ok && kernel.UnregisterForClientMessageEvent(clientCallbackID);

			if (!ok) {
				Debug.Log("Failed to unregister an event");
				return false;
			}

			// Close down the kernel (or for a remote connection disconnect from the kernel w/o closing it down).
			kernel.Shutdown();

			return ok;
		}

		static void MyRunEventCallback(smlRunEventId eventID, IntPtr callbackData, IntPtr agentPtr, smlPhase phase) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			String name = agent.GetAgentName();
			Debug.Log(eventID + " agent " + name + " in phase " + phase + " with user data " + userData);
		}

		static void MyProductionEventCallback(smlProductionEventId eventID, IntPtr callbackData, IntPtr agentPtr, String productionName, String instantiation) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			String name = agent.GetAgentName();
			Debug.Log(eventID + " agent " + name + " production " + productionName + " instantiation " + instantiation + " with user data " + userData);
		}

		static void MyXMLEventCallback(smlXMLEventId eventID, IntPtr callbackData, IntPtr agentPtr, IntPtr pXML) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			// Retrieve the xml object.  We don't own this object, so to keep it we'd need to copy it (or its contents).
			// This way when C# deallocated the object the underlying C++ object isn't deleted either (which is correct as the corresponding C++ code
			// doesn't ass ownership in the equivalent callback either).
			ClientXML xml = (ClientXML)((GCHandle)pXML).Target;

			// Convert the XML to string form so we can look at it.
			String xmlString = xml.GenerateXMLString(true);

			String name = agent.GetAgentName();
			Debug.Log(eventID + " agent " + name + " xml " + xmlString);
		}

		static void MyAgentEventCallback(smlAgentEventId eventID, IntPtr callbackData, IntPtr kernelPtr, String agentName) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			// This callback returns the name of the agent as a string, to avoid having SWIG have to lookup the C# agent object
			// and ass that back.  We do something similar in Java for the same reasons.
			Agent agent = kernel.GetAgent(agentName);
			
			if (agent == null)
				throw new Exception("Error looking up agent in callback");

			Debug.Log(eventID + " agent " + agentName + " with user data " + userData);
		}

		static void MySystemEventCallback(smlSystemEventId eventID, IntPtr callbackData, IntPtr kernelPtr) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			Debug.Log(eventID + " kernel " + kernel + " with user data " + userData);
		}

		static void MyUpdateEventCallback(smlUpdateEventId eventID, IntPtr callbackData, IntPtr kernelPtr, smlRunFlags runFlags) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			Debug.Log(eventID + " kernel " + kernel + " run flags " + runFlags + " with user data " + userData);
		}

		static void MyStringEventCallback(smlStringEventId eventID, IntPtr callbackData, IntPtr kernelPtr, String str) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			Debug.Log(eventID + " kernel " + kernel + " string " + str + " with user data " + userData);
		}

		static void MyPrintEventCallback(smlPrintEventId eventID, IntPtr callbackData, IntPtr agentPtr, String message) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			Debug.Log(eventID + " agent " + agent.GetAgentName() + " message " + message + " with user data " + userData);
		}

		static void MyOutputEventCallback(IntPtr callbackData, IntPtr agentPtr, String commandName, IntPtr outputWME) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			// Retrieve the wme.  We don't own this object, so to keep it we'd need to copy it (or its contents).
			// This way when C# deallocated the object the underlying C++ object isn't deleted either (which is correct as the corresponding C++ code
			// doesn't ass ownership in the equivalent callback either).
			WMElement wme = (WMElement)((GCHandle)outputWME).Target;

			String id  = wme.GetIdentifier().GetIdentifierSymbol();
			String att = wme.GetAttribute();
			String val = wme.GetValueAsString();

			Debug.Log("Output received under ^move attribute for agent " + agent.GetAgentName() + " output wme " + id + " ^" + att + " " + val);
		}

		static void MyOutputNotificationCallback(IntPtr callbackData, IntPtr agentPtr) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Agent agent = (Agent)((GCHandle)agentPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			Debug.Log("Agent " + agent.GetAgentName() + " has just received some output");

			for (int i = 0; i < agent.GetNumberOutputLinkChanges(); i++)
			{
				WMElement wme = agent.GetOutputLinkChange(i);
				Debug.Log("Output wme was " + wme.GetIdentifier().GetIdentifierSymbol() + " ^" + wme.GetAttribute() + " " + wme.GetValueAsString());
			}
		}

		static String MyTestRhsFunction(smlRhsEventId eventID, IntPtr callbackData, IntPtr kernelPtr, String agentName, String functionName, String argument) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			// This callback returns the name of the agent as a string, to avoid having SWIG have to lookup the C# agent object
			// and ass that back.  We do something similar in Java for the same reasons.
			Agent agent = kernel.GetAgent(agentName);
			
			if (agent == null)
				throw new Exception("Error looking up agent in callback");

			Debug.Log(eventID + " agent " + agentName + " function " + functionName + " arg " + argument);

			// This is the result of the RHS function and can be placed into working memory as the result of the call
			// (whether this happens or not depends on how the RHS function is used in the production rule).
			return "result";
		}

		static String MyTestClientMessageCallback(smlRhsEventId eventID, IntPtr callbackData, IntPtr kernelPtr, String agentName, String clientName, String message) {
			// Retrieve the original object reference from the GCHandle which is used to ass the value safely to and from C++ (unsafe/unmanaged) code.
			Kernel kernel = (Kernel)((GCHandle)kernelPtr).Target;

			// Retrieve arbitrary data from callback data object (note data here can be null, but the wrapper object callbackData won't be so this call is safe)
			// This field's usage is up to the user and assed in during registation call and assed back here.  Can be used to provide context.
			object userData = (object)((GCHandle)callbackData).Target;

			// This callback returns the name of the agent as a string, to avoid having SWIG have to lookup the C# agent object
			// and ass that back.  We do something similar in Java for the same reasons.
			Agent agent = kernel.GetAgent(agentName);
			
			if (agent == null)
				throw new Exception("Error looking up agent in callback");

			Debug.Log(eventID + " agent " + agentName + " client " + clientName + " message " + message);

			return "result";
		}
	}
}