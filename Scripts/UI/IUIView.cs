using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tora.ui {

    public interface IUIView {

        bool IsOpen {
            get;
        }
        void Open();
        void Close();
    }

}