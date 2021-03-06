﻿using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Core.Services;

namespace MultipleStartNodes.Trees
{
    [Tree("users", "userStartNodes", "User Start Nodes", sortOrder: 3)]
    [PluginController("MultipleStartNodes")]
    public class UserStartNodesController : TreeController
    {
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {                
                // root actions                
                menu.Items.Add<RefreshNode, ActionRefresh>(Services.TextService.Localize(string.Format("actions/{0}", ActionRefresh.Instance.Alias)), true);
            }

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            //check if we're rendering the root node's children
            if (id == Constants.System.Root.ToInvariantString())
            {
                int totalRecords;
                TreeNodeCollection nodes = new TreeNodeCollection();

                totalRecords = Services.UserService.GetCount(global::Umbraco.Core.Models.Membership.MemberCountType.All);
                IEnumerable<IUser> users = Services.UserService.GetAll(0, totalRecords, out totalRecords);

                foreach (IUser user in users)
                {
                    if (user.Id == 0)
                        continue;

                    TreeNode node = CreateTreeNode(
                        user.Id.ToInvariantString(),
                        "-1",
                        queryStrings,
                        user.Name,
                        "icon-user",
                        false         
                    );

                    node.MenuUrl = "";
                    
                    nodes.Add(node);
                }

                nodes.Sort((x,y) => string.Compare(x.Name, y.Name));

                return nodes;
            }

            //this tree doesn't support rendering more than 1 level
            throw new NotSupportedException();
        }
    }
}
