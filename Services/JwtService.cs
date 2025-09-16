using Microsoft.IdentityModel.Tokens;        // Contains classes for cryptographic keys and security tokens
using RestAPI.Models;                        
using System.IdentityModel.Tokens.Jwt;       // Contains JwtSecurityToken and JwtSecurityTokenHandler classes
using System.Security.Claims;                // Contains the Claim class for storing user information in tokens
using System.Text;                           // Contains Encoding class to convert strings to bytes

namespace RestAPI.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        
        public JwtService(IConfiguration config)
        {
            _secretKey = config["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey not found");
            _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer not found");
        }

        //creating a token when user logs in
        public string CreateToken(User user)
        {
            // SymmetricSecurityKey: This creates a cryptographic key from our secret string
            // "Symmetric" means the same key is used for both signing and verifying the token
            // We convert the secret key string to bytes because cryptographic operations work with bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            
            // SigningCredentials: This combines our key with a specific algorithm (HMAC-SHA256)
            // Think of this as choosing both the "lock" (key) and the "locking mechanism" (algorithm)
            // HMAC-SHA256 is a secure hashing algorithm that creates a digital signature
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims: These are pieces of information stored inside the token
            // Think of claims as "attributes" or "properties" about the user
            // They're called "claims" because the token is "claiming" these facts about the user
            // Important: Don't put sensitive data like passwords here - tokens can be decoded (but not modified without the secret key)
            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),        // Custom claim: stores user's ID
                new Claim("username", user.Username)            // Custom claim: stores username
            };

            // JwtSecurityToken: This is the actual JWT token object being created
            // A JWT has 3 parts: Header (algorithm info), Payload (claims), Signature (verification)
            var token = new JwtSecurityToken(
                issuer: _issuer,                                // Who created this token 
                audience: _issuer,                              // Who this token is intended for (usually same as issuer)
                claims: claims,                                 // The user information we're including
                expires: DateTime.Now.AddHours(1),             // When this token becomes invalid (security measure)
                signingCredentials: credentials                 // How to verify this token is authentic
            );

            // JwtSecurityTokenHandler: This converts our token object into a string
            // The string format is what gets sent to the client and used in API requests
            // Format: "header.payload.signature" 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}