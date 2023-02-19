using System;

namespace smlUnity {
    public class WMElement {
        protected IntPtr _pWMElement;

        public WMElement(IntPtr pWMElement) {
            _pWMElement = pWMElement;
        }

        public IntPtr GetPtr(){
            return _pWMElement;
        }
    }
}