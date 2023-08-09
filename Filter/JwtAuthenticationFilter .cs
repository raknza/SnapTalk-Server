using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using android_backend.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace android_backend.Filter;
public class JwtAuthenticationFilter : IAuthorizationFilter
{
    private readonly JwtHelper jwtHelper;

    public JwtAuthenticationFilter(JwtHelper jwtHelper)
    {
        this.jwtHelper = jwtHelper;
    }

    void IAuthorizationFilter.OnAuthorization(AuthorizationFilterContext context)
    {
        string jwtToken = null;
        string authorizationHeader = context.HttpContext.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            jwtToken = authorizationHeader.Substring("Bearer ".Length);
            string username = context.HttpContext.User.Identity.Name;
            bool isTokenValid = false;
            if(!string.IsNullOrEmpty(username)){
                isTokenValid = jwtHelper.IsInRedis(username,jwtToken);
            }
            if (!isTokenValid)
                context.HttpContext.Items["isTokenValid"] = false;
            else
                context.HttpContext.Items["isTokenValid"] = true;
        }
        else
        {
            context.HttpContext.Items["isTokenValid"] = false;
        }
    }
    
}