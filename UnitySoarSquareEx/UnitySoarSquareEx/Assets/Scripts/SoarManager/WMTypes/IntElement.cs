using System;

namespace smlUnity {
    public class IntElement: WMElement {
        public IntElement(IntPtr pIntElement) {
            _pWMElement = pIntElement;
        }
    }
}