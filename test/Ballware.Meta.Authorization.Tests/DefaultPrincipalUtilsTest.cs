using System.Security.Claims;
using Ballware.Meta.Authorization.Internal;
using Moq;

namespace Ballware.Meta.Authorization.Tests;

[TestFixture]
public class DefaultPrincipalUtilsTest
{
    [Test]
    public void Get_userid_from_principal_should_succeed()
    {
        var principalClaims = new List<Claim>();
        
        var expectedUserId = Guid.NewGuid();
        
        principalClaims.Add(new Claim("tenant", Guid.NewGuid().ToString()));
        principalClaims.Add(new Claim("non_relevant_claim_1", "fake value"));
        principalClaims.Add(new Claim("sub", expectedUserId.ToString()));
        principalClaims.Add(new Claim("non_relevant_claim_2", "fake value"));
        
        var principal = new ClaimsPrincipal(new ClaimsIdentity(principalClaims));

        var subject = new DefaultPrincipalUtils("tenant", "sub");

        var actualUserId = subject.GetUserId(principal);
        
        Assert.That(actualUserId, Is.EqualTo(expectedUserId));
    }
    
    [Test]
    public void Get_tenant_from_principal_should_succeed()
    {
        var principalClaims = new List<Claim>();
        
        var expectedTenantId = Guid.NewGuid();
        
        principalClaims.Add(new Claim("tenant", expectedTenantId.ToString()));
        principalClaims.Add(new Claim("non_relevant_claim_1", "fake value"));
        principalClaims.Add(new Claim("sub", Guid.NewGuid().ToString()));
        principalClaims.Add(new Claim("non_relevant_claim_2", "fake value"));
        
        var principal = new ClaimsPrincipal(new ClaimsIdentity(principalClaims));

        var subject = new DefaultPrincipalUtils("tenant", "sub");

        var actualTenantId = subject.GetUserTenandId(principal);
        
        Assert.That(actualTenantId, Is.EqualTo(expectedTenantId));
    }
    
    [Test]
    public void Get_claims_from_principal_should_succeed()
    {
        var principalClaims = new List<Claim>();
        
        var expectedTenantId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        
        principalClaims.Add(new Claim("tenant", expectedTenantId.ToString()));
        principalClaims.Add(new Claim("single_claim", "single value"));
        principalClaims.Add(new Claim("sub", expectedUserId.ToString()));
        principalClaims.Add(new Claim("list_claim", "list value 1"));
        principalClaims.Add(new Claim("list_claim", "list value 2"));
        
        var principal = new ClaimsPrincipal(new ClaimsIdentity(principalClaims));

        var subject = new DefaultPrincipalUtils("tenant", "sub");

        var actualClaims = subject.GetUserClaims(principal);
        
        Assert.Multiple(() =>
        {
            Assert.That(actualClaims.Count, Is.EqualTo(4));
            Assert.That(actualClaims["tenant"],  Is.EqualTo(expectedTenantId.ToString()));
            Assert.That(actualClaims["sub"],  Is.EqualTo(expectedUserId.ToString()));
            Assert.That(actualClaims["single_claim"], Is.EqualTo("single value"));
            Assert.That(actualClaims["list_claim"], Is.EquivalentTo(new string[] { "list value 1", "list value 2" }));
        });
    }
}