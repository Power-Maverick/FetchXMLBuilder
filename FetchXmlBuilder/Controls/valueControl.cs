﻿using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using Cinteros.Xrm.FetchXmlBuilder.DockControls;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Controls
{
    public partial class valueControl : FetchXmlElementControlBase
    {
        private string _entityName;
        private string _attributeName;

        public valueControl() : this(null, null, null)
        {
        }

        public valueControl(TreeNode node, FetchXmlBuilder fetchXmlBuilder, TreeBuilderControl tree)
        {
            InitializeComponent();
            InitializeFXB(null, fetchXmlBuilder, tree, node);

            _attributeName = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "attribute");
            _entityName = TreeNodeHelper.GetAttributeFromNode(Node.Parent, "entity");

            if (String.IsNullOrWhiteSpace(_entityName))
            {
                _entityName = TreeNodeHelper.GetAttributeFromNode(Node.Parent.Parent.Parent, "name");
            }
            else
            {
                // TODO: Entity is an alias, get the actual entity name
            }

            if (fxb.NeedToLoadEntity(_entityName))
            {
                if (!fxb.working)
                {
                    fxb.LoadEntityDetails(_entityName, RefreshValues);
                }
            }
            else
            {
                RefreshValues();
            }
        }

        private void RefreshValues()
        {
            cmbValue.Items.Clear();
            cmbValue.DropDownStyle = ComboBoxStyle.Simple;

            var entities = fxb.GetDisplayEntities();
            if (entities != null && entities.ContainsKey(_entityName))
            {
                var entity = entities[_entityName];
                var attribute = entity.Attributes.SingleOrDefault(a => a.LogicalName == _attributeName);

                if (attribute != null)
                {
                    // Show correct editor based on type of attribute
                    if (attribute is EnumAttributeMetadata enummeta &&
                         enummeta.OptionSet is OptionSetMetadata options &&
                         !(attribute is EntityNameAttributeMetadata))
                    {
                        cmbValue.Items.AddRange(options.Options.Select(o => new OptionsetItem(o)).ToArray());

                        if (cmbValue.Items.Count > 0 && cmbValue.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(cmbValue.Text))
                        {
                            var item = cmbValue.Items.OfType<OptionsetItem>().FirstOrDefault(i => i.GetValue() == cmbValue.Text);
                            cmbValue.SelectedItem = item;
                        }

                        cmbValue.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                    else if (attribute is LookupAttributeMetadata lookupmeta)
                    {
                        // TODO: Show lookup control
                    }
                }
            }
        }
    }
}