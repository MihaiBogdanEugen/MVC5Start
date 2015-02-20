using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MVC5Start.ViewModels;
using Constants = MVC5Start.Infrastructure.Constants;

namespace MVC5Start.Controllers.Definitions
{
    public abstract class BaseController : Controller
    {
        #region Properties

        private int _userId;
        protected int UserId
        {
            get
            {
                if (this._userId == 0)
                    this._userId = this.User.Identity.IsAuthenticated ? this.User.Identity.GetUserId<int>() : 0;
                return this._userId;
            }
        }

        protected string UserIdAsString
        {
            get
            {
                return this._userId.ToString(CultureInfo.InvariantCulture);
            }
        }

        #endregion Properties

        #region Helpers

        protected void AddModelErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
                this.ModelState.AddModelError("", error);
        }

        protected void AddModelError(string error)
        {
            this.ModelState.AddModelError("", error);
        }
        
        protected string GetCallbackUrl(string actionName, string controllerName, int userId, string code)
        {
            return this.Request.Url == null 
                ? this.Url.Action(actionName, controllerName, new {userId, code }) 
                : this.Url.Action(actionName, controllerName, new {userId, code }, this.Request.Url.Scheme);
        }

        public string GetUrl(string actionName, string controllerName)
        {
            return this.Request.Url == null
                ? this.Url.Action(actionName, controllerName)
                : this.Url.Action(actionName, controllerName, null, this.Request.Url.Scheme);
        }

        #endregion Helpers

        #region Redirects

        protected ActionResult RedirectToInvalidModel(string viewName, object model, IEnumerable<string> errors)
        {
            this.AddModelErrors(errors);
            return this.View(viewName, model);        
        }

        protected ActionResult RedirectToInvalidModel(string viewName, object model, params string[] errors)
        {
            this.AddModelErrors(errors);
            return this.View(viewName, model);        
        }

        protected ActionResult RedirectToInvalidModel(object model, IEnumerable<string> errors)
        {
            this.AddModelErrors(errors);
            return this.View(model);                
        }

        protected ActionResult RedirectToInvalidModel(object model, params string[] errors)
        {
            this.AddModelErrors(errors);
            return this.View(model);                
        }

        protected ActionResult RedirectToError(string format, params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, args);

            return this.RedirectToError(message);
        }

        protected ActionResult RedirectToError(string message)
        {
            MvcApplication.LogException(new ApplicationException(message));

            return this.View("Error", new ErrorViewModel {Message = message});
        }

        protected ActionResult RedirectToLocalUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || this.Url.IsLocalUrl(returnUrl) == false)
                return this.RedirectToHome;

            return this.Redirect(returnUrl);
        }

        protected ActionResult RedirectToHome
        {
            get
            {
                return this.RedirectToAction(Constants.DefaultAction, Constants.DefaultController);
            }
        }

        #endregion Redirects

        #region Settings SelectLists

        public SelectListItem[] GetSelectListDateFormats(string selectedValue)
        {
            return Constants.DateFormats.Select(x => new SelectListItem
            {
                Text = x, 
                Value = x, 
                Selected = x == (string.IsNullOrEmpty(selectedValue) ? Constants.DefaultDateFormat : selectedValue) 
            }).ToArray();
        }   

        public SelectListItem[] GetSelectListTimeFormats(string selectedValue)
        {
            return Constants.TimeFormats.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = x == (string.IsNullOrEmpty(selectedValue) ? Constants.DefaultTimeFormat : selectedValue)
            }).ToArray();
        }

        public SelectListItem[] GetSelectListTimeZones(string selectedValue)
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(x => new SelectListItem
            {
                Text = x.DisplayName,
                Value = x.DisplayName,
                Selected = x.DisplayName == (string.IsNullOrEmpty(selectedValue) ? Constants.DefaultTimeZoneCode : selectedValue)
            }).ToArray();
        }

        public SelectListItem[] GetSelectListLanguages(string selectedValue)
        {
            return Constants.Languages.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key,
                Selected = x.Key == (string.IsNullOrEmpty(selectedValue) ? Constants.DefaultLanguageCode : selectedValue) 
            }).ToArray();
        }

        #endregion Settings SelectLists
    }
}