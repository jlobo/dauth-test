using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace dAuthMe.api.Tools
{
    public class JwtToken
    {
        const string rex = "[Bb]earer\\s+([a-zA-Z0-9\\-_]+\\.[a-zA-Z0-9\\-_]+(?:\\.[a-zA-Z0-9\\-_]+)?)";
        private enum KeyType {ECDsa = 1, Hmac = 2};

        private readonly SecurityKey _key;
        private readonly KeyType _type;
        private readonly Dictionary<string, Claim> _claims;

        public IEnumerable<Claim> Claims => _claims.Values;

        private JwtToken(SecurityKey key, KeyType type) {
            _key = key;
            _type = type;
            _claims = new Dictionary<string,Claim>();
        }

        public static JwtToken FromECDsa(ECDsa key) =>
            new JwtToken(new ECDsaSecurityKey(key), KeyType.ECDsa);

        public static JwtToken FromECDsa(byte[] key, bool isPrivate) =>
            new JwtToken(new ECDsaSecurityKey(ECDsaSerializer.GetEcdsa(key, isPrivate)), KeyType.ECDsa);

        public static JwtToken FromECDsa(string key, bool isPrivate) =>
            new JwtToken(new ECDsaSecurityKey(ECDsaSerializer.GetEcdsa(key, isPrivate)), KeyType.ECDsa);

        public static JwtToken FromHmac(byte[] key) =>
            new JwtToken(new SymmetricSecurityKey(key), KeyType.Hmac);

        public static JwtToken FromHmac(string key) =>
            new JwtToken(new SymmetricSecurityKey(Convert.FromBase64String(key)), KeyType.Hmac);

        public JwtToken AddSubject(string subject) => AddClaims(new Claim("sub", subject));

        public JwtToken AddIssuer(string issuer) => AddClaims(new Claim("iss", issuer));

        public JwtToken AddAudience(string audience) => AddClaims(new Claim("aud", audience));
        
        public JwtToken AddClaims(params Claim[] claims) => AddClaims(claims as IEnumerable<Claim>);

        public JwtToken AddClaims(IEnumerable<Claim> claims) {
            foreach (var claim in claims)
            {
                _claims[claim.Type] = claim;
            }
            
            return this;
        }

        public bool Verify(string token) {
            var toValidate = new TokenValidationParameters {
                IssuerSigningKey = _key,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = _claims.ContainsKey("iss"),
                ValidateAudience = _claims.ContainsKey("aud")
            };

            toValidate.ValidIssuer = toValidate.ValidateIssuer ? _claims["iss"].Value : null;
            toValidate.ValidAudience = toValidate.ValidateAudience ? _claims["aud"].Value : null;

            try { new JwtSecurityTokenHandler().ValidateToken(token, toValidate, out var validated); }
            catch { return false; }

            AddClaims(new JwtSecurityToken(token).Claims);
            return true;
        }

        public string Generate(TimeSpan expires) {
            var descriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.Add(expires),
                IssuedAt = DateTime.Now,
                SigningCredentials = GetSignatureKey()
            };
            
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(descriptor));
        }

        private SigningCredentials GetSignatureKey()
        {
            switch (_type)
            {
                case KeyType.ECDsa: return new SigningCredentials(_key, SecurityAlgorithms.EcdsaSha256);
                case KeyType.Hmac: return new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            }
            
            throw new NotImplementedException($"'{_type}' is not implemented");
        }

        public static bool TryParse(string token, out JwtSecurityToken sec) {
            var match = Regex.Match(token, rex);
            if (match.Groups.Count < 2) {
                sec = null;
                return false;
            }

            sec = new JwtSecurityToken(match.Groups[1].Value);
            return true;
        }
    }
}