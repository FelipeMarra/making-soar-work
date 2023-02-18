using System;

namespace smlUnity {
    public class WMElement {
        protected IntPtr _pWMElement;

        public IntPtr GetPtr(){
            return _pWMElement;
        }
    }
}