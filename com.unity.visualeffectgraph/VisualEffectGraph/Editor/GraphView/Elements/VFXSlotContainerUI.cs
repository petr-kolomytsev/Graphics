using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Experimental.UIElements.StyleEnums;
using System.Reflection;
using System.Linq;

namespace UnityEditor.VFX.UI
{
    class VFXSlotContainerUI : VFXNodeUI
    {
        public VisualElement m_SettingsContainer;

        public bool collapse
        {
            get { return GetPresenter<VFXNodePresenter>().model.collapsed; }

            set
            {
                if (GetPresenter<VFXNodePresenter>().model.collapsed != value)
                {
                    GetPresenter<VFXNodePresenter>().model.collapsed = value;
                }
            }
        }

        public VFXSlotContainerUI()
        {
            this.AddManipulator(new Collapser());
        }

        public override void OnDataChanged()
        {
            base.OnDataChanged();
            var presenter = GetPresenter<VFXSlotContainerPresenter>();

            if (presenter == null)
                return;

            if (m_SettingsContainer == null && presenter.settings != null)
            {
                object settings = presenter.settings;

                m_SettingsContainer = new VisualElement { name = "settings" };

                leftContainer.Insert(1, m_SettingsContainer); //between title and input

                foreach (var setting in presenter.settings)
                {
                    AddSetting(setting);
                }
            }
            if (m_SettingsContainer != null)
            {
                for (int i = 0; i < m_SettingsContainer.childCount; ++i)
                {
                    PropertyRM prop = m_SettingsContainer.ElementAt(i) as PropertyRM;
                    if (prop != null)
                        prop.Update();
                }
            }

            GraphView graphView = this.GetFirstAncestorOfType<GraphView>();
            if (graphView != null)
            {
                var allEdges = graphView.Query<Edge>().ToList();

                foreach (NodeAnchor anchor in this.Query<NodeAnchor>().Where(t => true).ToList())
                {
                    foreach (var edge in allEdges.Where(t =>
                        {
                            var pres = t.GetPresenter<EdgePresenter>();
                            return pres != null && (pres.output == anchor.presenter || pres.input == anchor.presenter);
                        }))
                    {
                        edge.OnDataChanged();
                    }
                }
            }


            if (presenter.model.collapsed)
            {
                AddToClassList("collapsed");
            }
            else
            {
                RemoveFromClassList("collapsed");
            }
        }

        protected void AddSetting(VFXSettingPresenter setting)
        {
            var rm = PropertyRM.Create(setting, 100);
            if (rm != null)
            {
                m_SettingsContainer.Add(rm);
            }
            else
            {
                Debug.LogErrorFormat("Cannot create presenter for {0}", setting.name);
            }
        }
    }
}
