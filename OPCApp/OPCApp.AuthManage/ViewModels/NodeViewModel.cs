using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Domain.Models;

namespace OPCApp.AuthManage
{
    public class NodeViewModel : BindableBase
    {
        /*
         * 省去构造函数，公共方法和一些静态成员
         * */
        public NodeViewModel(NodeInfo info)
        {
            children = new ObservableCollection<NodeViewModel>();
            NodeInfo = info;
        }

        public NodeViewModel(NodeViewModel parent, OPC_OrgInfo orgInfo)
        {
            Parent = parent;
            Name = orgInfo.OrgName;
            ParentId = orgInfo.ParentID;
            OrgId = orgInfo.OrgID;
            OrgType = orgInfo.OrgType ;
            StoreOrSectionId = orgInfo.StoreOrSectionID;
            StoreOrSectionName = orgInfo.StoreOrSectionName;
            NodeInfo = parent.NodeInfo;
            children = new ObservableCollection<NodeViewModel>();

        }
        private NodeViewModel(NodeViewModel parent, string name,string parentID,int orgType,int? storeOrSectionId,string storeOrSectionName)
        {
            Parent = parent;
            Name = name;
            ParentId = parentID;
            OrgType = orgType;
            StoreOrSectionId = storeOrSectionId;
            StoreOrSectionName = storeOrSectionName;

            NodeInfo = parent.NodeInfo;
            children = new ObservableCollection<NodeViewModel>();
        }

        #region 字段

        string _name;
        private string _orgId;
        private int? _orgType;
        private string _parentId;
        private int? _storeOrSectionId;
        private string _storeOrSectionName;
        bool _isExpanded;
        bool _isSelected;
         ObservableCollection<NodeViewModel> children;

        #endregion


        #region 属性

        public NodeInfo NodeInfo { get; private set; }
        public NodeViewModel Parent { get; private set; }
        public string StoreOrSectionName
        {
            get
            {
                return _storeOrSectionName;
            }
            set
            {
                if (_storeOrSectionName != value)
                {
                    _storeOrSectionName = value;
                    OnPropertyChanged("StoreOrSectionName");
                }
            }
        }
        public int? StoreOrSectionId
        {
            get
            {
                return _storeOrSectionId;
            }
            set
            {
                if (_storeOrSectionId != value)
                {
                    _storeOrSectionId = value;
                    OnPropertyChanged("StoreOrSectionId");
                }
            }
        }
        public int? OrgType
        {
            get
            {
                return _orgType;
            }
            set
            {
                if (_orgType != value)
                {
                    _orgType = value;
                    OnPropertyChanged("OrgType");
                }
            }
        }
        public string ParentId
        {
            get
            {
                return _parentId;
            }
            set
            {
                if (_parentId != value)
                {
                    _parentId = value;
                    OnPropertyChanged("ParentId");
                }
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string OrgId
        {
            get
            {
                return _orgId;
            }
            set
            {
                if (_orgId != value)
                {
                    _orgId = value;
                    OnPropertyChanged("OrgId");
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnIsExpandedChanged();
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }

        public ObservableCollection<NodeViewModel> Children
        {
            get
            {
                return new ObservableCollection<NodeViewModel>(children);
            }
        }


        #endregion


        #region 方法

        public void Remove()
        {
            if (Parent != null)
            {
                Parent.children.Remove(this);
                RefreshInfoCount(-1);
            }
        }

        public void Update()
        {

        }

        public NodeViewModel AddSubNode(OPC_OrgInfo opcOrgInfo)
        {
            IsExpanded = true;
            var node = new NodeViewModel(this, opcOrgInfo);
            AddNodeViewModel(node);
            return node;
        }

        public void AddNodeViewModel(NodeViewModel nv)
        {
            children.Insert(0,nv);
        }

        public void Append(OPC_OrgInfo opcOrgInfo)
        {
            IsExpanded = true;
            children.Add(new NodeViewModel(this, opcOrgInfo));
            RefreshInfoCount(1);
        }
        public void Rename()
        {
            Name = ""; //GetNextDataName();
        }

        public void MoveUp()
        {
            if (Parent != null)
            {
                var idx = Parent.children.IndexOf(this);
                if (idx > 0)
                {
                    Parent.children.RemoveAt(idx);
                    Parent.children.Insert(--idx, this);
                }
                IsSelected = true;
            }
        }
        public void MoveDown()
        {
            if (Parent != null)
            {
                var idx = Parent.children.IndexOf(this);
                if (idx < Parent.children.Count - 1)
                {
                    Parent.children.RemoveAt(idx);
                    Parent.children.Insert(++idx, this);
                }
                IsSelected = true;
            }
        }

        #endregion

        #region 私有方法

        void RefreshInfoCount(int addition)
        {
            //if (NodeInfo != null)
            //    //NodeInfo.SetCount(NodeInfo.Count + addition);
            //    return;
        }

        #endregion

        #region 事件

        public event EventHandler IsExpandedChanged;
        public event EventHandler IsSelectedChanged;

        protected virtual void OnIsExpandedChanged()
        {
            if (IsExpandedChanged != null)
                IsExpandedChanged(this, EventArgs.Empty);
        }

        protected virtual void OnIsSelectedChanged()
        {
            if (IsSelectedChanged != null)
                IsSelectedChanged(this, EventArgs.Empty);
            if (IsSelected)
                NodeInfo.SelectedNode = this;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string proName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(proName));
        }

        #endregion
    }

    public class NodeInfo : BindableBase
    {
        NodeViewModel _selectedNode;
        public NodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    OnSelectedNodeChanged();
                    OnPropertyChanged("SelectedNode");
                }
            }
        }
      

        public event EventHandler SelectedNodeChanged;

        protected virtual void OnSelectedNodeChanged()
        {
            if (SelectedNodeChanged != null)
                SelectedNodeChanged(this, EventArgs.Empty);
        }


        #region INotifyPropertyChanged Members

        public new event PropertyChangedEventHandler PropertyChanged;

        protected new virtual void OnPropertyChanged(string proName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(proName));
        }

        #endregion
    }
}
