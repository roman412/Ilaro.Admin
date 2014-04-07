﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ilaro.Admin.Views.IlaroAdmin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Ilaro.Admin.Commons.Paging;
    using Ilaro.Admin.Extensions;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/IlaroAdmin/Edit.cshtml")]
    public partial class Edit : System.Web.Mvc.WebViewPage<Ilaro.Admin.ViewModels.EditViewModel>
    {
        public Edit()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\IlaroAdmin\Edit.cshtml"
  
    Layout = "~/Views/IlaroAdmin/_Layout.cshtml";
    ViewBag.Title = "Edit";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Breadcrumb", () => {

WriteLiteral("\r\n    <ul");

WriteLiteral(" class=\"breadcrumb\"");

WriteLiteral(">\r\n        <li>");

            
            #line 11 "..\..\Views\IlaroAdmin\Edit.cshtml"
       Write(Html.ActionLink("Administracja", "Index"));

            
            #line default
            #line hidden
WriteLiteral(" <span");

WriteLiteral(" class=\"divider\"");

WriteLiteral(">/</span></li>\r\n        <li>");

            
            #line 12 "..\..\Views\IlaroAdmin\Edit.cshtml"
       Write(Html.ActionLink(Model.Entity.GroupName, "Group", new { groupName = Model.Entity.GroupName }));

            
            #line default
            #line hidden
WriteLiteral(" <span");

WriteLiteral(" class=\"divider\"");

WriteLiteral(">/</span></li>\r\n        <li>");

            
            #line 13 "..\..\Views\IlaroAdmin\Edit.cshtml"
       Write(Html.ActionLink(Model.Entity.Plural, "Details", new { entityName = Model.Entity.Name }));

            
            #line default
            #line hidden
WriteLiteral(" <span");

WriteLiteral(" class=\"divider\"");

WriteLiteral(">/</span></li>\r\n        <li");

WriteLiteral(" class=\"active\"");

WriteLiteral(">Editing</li>\r\n    </ul>\r\n");

});

WriteLiteral("\r\n<h2>");

            
            #line 18 "..\..\Views\IlaroAdmin\Edit.cshtml"
Write(Model.Entity.Singular);

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n\r\n");

            
            #line 20 "..\..\Views\IlaroAdmin\Edit.cshtml"
 using (Html.BeginForm("Edit", "IlaroAdmin", FormMethod.Post, new { @class = "form-horizontal" }))
{
    
            
            #line default
            #line hidden
            
            #line 22 "..\..\Views\IlaroAdmin\Edit.cshtml"
Write(Html.AntiForgeryToken());

            
            #line default
            #line hidden
            
            #line 22 "..\..\Views\IlaroAdmin\Edit.cshtml"
                            
    
            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\IlaroAdmin\Edit.cshtml"
Write(Html.ValidationSummary(true));

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\IlaroAdmin\Edit.cshtml"
                                 

    if (Model.PropertiesGroups.Count > 1)
    {
        foreach (var group in Model.PropertiesGroups)
        {

            
            #line default
            #line hidden
WriteLiteral("            <fieldset>\r\n                <legend>");

            
            #line 30 "..\..\Views\IlaroAdmin\Edit.cshtml"
                   Write(group.GroupName);

            
            #line default
            #line hidden
WriteLiteral(" <button");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"btn pull-right\"");

WriteLiteral("><i");

WriteAttribute("class", Tuple.Create(" class=\"", 1052), Tuple.Create("\"", 1133)
            
            #line 30 "..\..\Views\IlaroAdmin\Edit.cshtml"
                , Tuple.Create(Tuple.Create("", 1060), Tuple.Create<System.Object, System.Int32>(Html.Condition(group.IsCollapsed, () => "icon-plus", () => "icon-minus")
            
            #line default
            #line hidden
, 1060), false)
);

WriteLiteral("></i></button></legend>\r\n                <div");

WriteAttribute("class", Tuple.Create(" class=\"", 1179), Tuple.Create("\"", 1242)
, Tuple.Create(Tuple.Create("", 1187), Tuple.Create("fields", 1187), true)
            
            #line 31 "..\..\Views\IlaroAdmin\Edit.cshtml"
, Tuple.Create(Tuple.Create(" ", 1193), Tuple.Create<System.Object, System.Int32>(Html.Condition(group.IsCollapsed, () => "hide")
            
            #line default
            #line hidden
, 1194), false)
);

WriteLiteral(">\r\n");

            
            #line 32 "..\..\Views\IlaroAdmin\Edit.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 32 "..\..\Views\IlaroAdmin\Edit.cshtml"
                     foreach (var property in group.Properties)
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"control-group\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 35 "..\..\Views\IlaroAdmin\Edit.cshtml"
                       Write(Html.EditorFor(m => property, property.EditorTemplateName));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n");

            
            #line 37 "..\..\Views\IlaroAdmin\Edit.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </div>\r\n            </fieldset>\r\n");

            
            #line 40 "..\..\Views\IlaroAdmin\Edit.cshtml"
        }
    }
    else if (Model.PropertiesGroups.Count == 1)
    {
        foreach (var property in Model.PropertiesGroups[0].Properties)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"control-group\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 47 "..\..\Views\IlaroAdmin\Edit.cshtml"
           Write(Html.EditorFor(m => property, property.EditorTemplateName));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 49 "..\..\Views\IlaroAdmin\Edit.cshtml"
        }
    }

    
            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\IlaroAdmin\Edit.cshtml"
Write(Html.Hidden("EntityName", Model.Entity.Name));

            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\IlaroAdmin\Edit.cshtml"
                                                 


            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"form-actions\"");

WriteLiteral(">\r\n        <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn btn-primary\"");

WriteLiteral(">Save</button>\r\n        <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn\"");

WriteLiteral(" name=\"ContinueEdit\"");

WriteLiteral("><i");

WriteLiteral(" class=\"icon-edit\"");

WriteLiteral("></i>Save and continue edit</button>\r\n        <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn\"");

WriteLiteral(" name=\"AddNext\"");

WriteLiteral("><i");

WriteLiteral(" class=\"icon-plus\"");

WriteLiteral("></i>Save and add next</button>\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2289), Tuple.Create("\"", 2358)
            
            #line 58 "..\..\Views\IlaroAdmin\Edit.cshtml"
, Tuple.Create(Tuple.Create("", 2296), Tuple.Create<System.Object, System.Int32>(Url.Action("Details", new { entityName = Model.Entity.Name })
            
            #line default
            #line hidden
, 2296), false)
);

WriteLiteral(" class=\"btn btn-link\"");

WriteLiteral(">Cancel</a>\r\n        <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2403), Tuple.Create("\"", 2501)
            
            #line 59 "..\..\Views\IlaroAdmin\Edit.cshtml"
, Tuple.Create(Tuple.Create("", 2410), Tuple.Create<System.Object, System.Int32>(Url.Action("Delete", new { entityName = Model.Entity.Name, key = Model.Entity.Key.Value })
            
            #line default
            #line hidden
, 2410), false)
);

WriteLiteral(" class=\"btn btn-danger pull-right\"");

WriteLiteral(">Delete</a>\r\n    </div>\r\n");

            
            #line 61 "..\..\Views\IlaroAdmin\Edit.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
