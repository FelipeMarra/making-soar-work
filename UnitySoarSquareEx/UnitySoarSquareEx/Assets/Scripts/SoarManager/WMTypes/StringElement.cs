using System;
using smlUnity;

namespace smlUnity {
    public class StringElement: WMElement{
        public StringElement(IntPtr pStringElement) {
            _pWMElement = pStringElement;
        }
    }
}