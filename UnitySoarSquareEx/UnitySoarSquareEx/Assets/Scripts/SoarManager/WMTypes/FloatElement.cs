using System;
using smlUnity;

namespace smlUnity {
    public class FloatElement: WMElement {
        public FloatElement(IntPtr pFloatElement) {
            _pWMElement = pFloatElement;
        }
    }
}
