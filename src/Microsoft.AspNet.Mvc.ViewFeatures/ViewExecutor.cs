// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Internal;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Utility type for rendering a <see cref="IView"/> to the response.
    /// </summary>
    public static class ViewExecutor
    {
        public static readonly MediaTypeHeaderValue DefaultContentType = new MediaTypeHeaderValue("text/html")
        {
            Encoding = Encoding.UTF8
        }.CopyAsReadOnly();

        /// <summary>
        /// Asynchronously renders the specified <paramref name="view"/> to the response body.
        /// </summary>
        /// <param name="view">The <see cref="IView"/> to render.</param>
        /// <param name="actionContext">The <see cref="ActionContext"/> for the current executing action.</param>
        /// <param name="viewData">The <see cref="ViewDataDictionary"/> for the view being rendered.</param>
        /// <param name="tempData">The <see cref="ITempDataDictionary"/> for the view being rendered.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous rendering.</returns>
        public static async Task ExecuteAsync(IView view,
                                              ActionContext actionContext,
                                              ViewDataDictionary viewData,
                                              ITempDataDictionary tempData,
                                              HtmlHelperOptions htmlHelperOptions,
                                              MediaTypeHeaderValue contentType)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (tempData == null)
            {
                throw new ArgumentNullException(nameof(tempData));
            }

            if (htmlHelperOptions == null)
            {
                throw new ArgumentNullException(nameof(htmlHelperOptions));
            }

            var response = actionContext.HttpContext.Response;

            contentType = contentType ?? DefaultContentType;
            if (contentType.Encoding == null)
            {
                // Do not modify the user supplied content type, so copy it instead
                contentType = contentType.Copy();
                contentType.Encoding = Encoding.UTF8;
            }

            response.ContentType = contentType.ToString();

            using (var writer = new HttpResponseStreamWriter(response.Body, contentType.Encoding))
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewData,
                    tempData,
                    writer,
                    htmlHelperOptions);

                await view.RenderAsync(viewContext);
            }
        }
    }
}