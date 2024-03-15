using halloDocEntities.DataModels;
using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(Aspnetuser aspuser);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        public JwtClaimsModel GetClaimsFromJwtToken(string token);

        public string encry(string pass);

        public string decry(string pass);
    }
}
