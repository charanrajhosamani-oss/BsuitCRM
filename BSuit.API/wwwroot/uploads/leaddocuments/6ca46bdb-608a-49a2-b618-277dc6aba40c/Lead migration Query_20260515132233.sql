INSERT INTO [BSuit].[CRM].[Leads]
(
    LeadId,
    TenantId,
    FirstName, -- Done
    LastName, -- Done
    EnquiryId,
    Email, -- Done
    Phone, -- Done
    CompanyName, -- Done
    City, -- Done
    ZipCode, -- Done
    Website, -- Done
    CountryId,
    PersonalEmail1, -- Done
    PersonalEmail2, -- Done
    Address, -- Done
    RequirementDetails,
    SkpeId,
    TwitterURL,
    FacebookURL,
    LinkedinURL,
    BDComments,
    PreferredModeOfCommunication,
    CustomerBackground,
    CustomerTypeId,
    CompanySizeId,
    CompanyRevenue,
    CompanyRanking,
    RatingId,
    LeadPriorityId,
    GenderId,
    JobTitleId,
    IndustryId,
    LeadTypeId,
    LeadSourceId,
    LeadStageId,
    OwnerId,
    IsActive,
    CreatedBy,
    CreatedOn
)
SELECT
    NEWID() AS LeadId,   -- 🔥 Generates unique GUID for each row
    '6200E0AE-F4C7-4509-B618-DC3490EE88D1' AS TenantId,
    LEFT(Client_Name, CHARINDEX(' ', Client_Name + ' ') - 1) AS FirstName,
    STUFF(Client_Name, 1, CHARINDEX(' ', Client_Name + ' '), '') AS LastName,
    Customer_ID as EnquiryId,
    Email,
    Phone,
    Company_Account,
    City,
    Zip_Postal_Code,
    Website,
    NULL as CountryId, -- Need to map
    Personal_Email_ID_1,
    Personal_Email_ID_2,
    Street AS Address,
    Lead_Comments AS RequirementDetails,
    Skype_ID,
    Twitter_URL,
    Facebook_URL,
    LinkedIn_URL,
    NC_Executive_Comments as BDComments,
    Preferred_Mode_Of_Communication as PreferredModeOfCommunication,
    NULL as CustomerBackground,
    CASE
    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'consultant'
        THEN '974796F2-AB0E-492C-85BA-0A62AE2E1B3D'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'consulting firm'
        THEN 'E8C99B37-5675-4324-8DC2-7A231A0B6447'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'educational institution'
        THEN 'CFD446AD-E891-42C2-8B37-C5F4497AA7D7'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'enterprise'
        THEN '80A5A98C-6139-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'entrepreneur'
        THEN 'FA7BAD97-6139-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'fortune 500'
        THEN 'D1274AB1-8C47-F111-0437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'government body'
        THEN '6D6AE29E-6139-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'individual'
        THEN '6E6AE29E-6139-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'large enterprise'
        THEN '2D2AC68B-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'micro business'
        THEN 'D1274AB1-8C47-F111-7437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'non profit'
        THEN '53600298-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'others'
        THEN 'D1074AB1-8C47-F111-7437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'professional'
        THEN '54600298-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'small business'
        THEN '426593A0-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'smb'
        THEN 'F4CBBFA6-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'startup'
        THEN 'D0274AB1-8C47-F111-8437-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(Customer_Type))) = 'student'
        THEN 'D1274AB1-8C47-F111-8437-D404E6DFC771'

    WHEN Customer_Type IS NULL OR LTRIM(RTRIM(Customer_Type)) = ''
        THEN NULL

    ELSE NULL
END AS CustomerTypeId,

    CASE
    WHEN TRY_CAST(No_of_Employees AS INT) BETWEEN 1 AND 50
        THEN '63CEF53C-44FE-474D-B460-33D042D45A3A'

    WHEN TRY_CAST(No_of_Employees AS INT) >= 1000
        THEN '0AE55CAB-BF80-4AB4-9A28-B564FB62F580'

    WHEN No_of_Employees IS NULL OR LTRIM(RTRIM(No_of_Employees)) = ''
        THEN NULL  -- or default if needed

    ELSE NULL
END AS CompanySizeId,
    Annual_Revenue,
    Fortune_Ranking,
    NULL as RatingId,
    NULL as LeadPriorityId,
    CASE
    WHEN LTRIM(RTRIM(LOWER(Gender))) = 'male'
        THEN '2FC9FC71-F6B3-4F38-BCE0-AE3F91E420FA'

    WHEN LTRIM(RTRIM(LOWER(Gender))) = 'female'
        THEN 'F08C05E0-58C4-4E8D-9DEB-9D98792A7932'

    WHEN Gender IS NULL OR LTRIM(RTRIM(Gender)) = ''
        THEN NULL  -- or set default if needed

    ELSE NULL  -- fallback
END AS GenderId,
    NULL as JobTitleId,
    NULL as IndustryId,
    CASE
    WHEN LTRIM(RTRIM(LOWER(Lead_Source))) = 'phone'
        THEN '39905FC4-E705-4DBE-9029-8A9AFA51B3D4'

    WHEN LTRIM(RTRIM(LOWER(Lead_Source))) = 'email'
        THEN '3C51F466-3B77-4530-BB95-E8042F0DC013'

    WHEN LTRIM(RTRIM(LOWER(Lead_Source))) IN ('rfi', 'quick submit')
        THEN '9E4C58DF-3D2E-44FB-88C2-3C1C617EC35B'

    ELSE NULL
END AS LeadTypeId,
    CASE
    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'tim ferriss - the 4 hour workweek'
        THEN '2AF59B42-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'thomas friedman - the world is flat'
        THEN '29F59B42-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'linkedin'
        THEN 'E2F2A66B-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'greetings email'
        THEN 'C4CCC872-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'facebook'
        THEN 'C8E00484-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'referral'
        THEN '10779139-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) IN ('search engine', 'search engine ')
        THEN 'DF06902E-CA26-F111-8435-D404E6DFC771'

    WHEN LTRIM(RTRIM(LOWER(How_did_you_hear_about_Brickwork))) = 'others'
        THEN '3EF9B4B6-9E47-F111-8437-D404E6DFC771'

    WHEN How_did_you_hear_about_Brickwork IS NULL OR LTRIM(RTRIM(How_did_you_hear_about_Brickwork)) = ''
        THEN '3EF9B4B6-9E47-F111-8437-D404E6DFC771' -- default Others

    ELSE '3EF9B4B6-9E47-F111-8437-D404E6DFC771' -- fallback
END AS LeadSourceId,
    CASE 
    WHEN LTRIM(RTRIM(LOWER(Lead_Stage))) IN (
        'wrong information',
        'Wrong Information - Auto Reject',
        'legal challenges / regulatory challenges',
        'language - auto reject'
    ) THEN 'E964A7E2-F740-4A31-AF28-9E98BA47186B'  -- Reject

    WHEN LTRIM(RTRIM(LOWER(Lead_Stage))) IN (
        'email follow up 1',
        'email follow up 2',
        'scoping call 1',
        'initial response'
    ) THEN 'FA6D51CB-D292-477A-8B82-0C75BE5BF94F'  -- Contacted

    WHEN LTRIM(RTRIM(LOWER(Lead_Stage))) = 'ready for estimation'
        THEN '647D552B-7249-43A4-A535-4D51BC9EED2A' -- Opportunity

    WHEN Lead_Stage IS NULL 
         OR LTRIM(RTRIM(Lead_Stage)) = ''
        THEN '29B63EC2-5BC7-4404-B006-D3E5149D8757'  -- New

    ELSE NULL
END AS LeadStageId,
    NULL as OwnerId,
     1 AS IsActive,
     NULL as CreatedBy,
    Create_Date
FROM [BSuit].[CRM].[Lead_Report_Sample];

-- TRUNCATE TABLE CRM.Leads;

-- DELETE FROM CRM.Leads;

-- TRUNCATE TABLE [CRM].[LeadServiceMapping];


SELECT * FROM [BSuit].[CRM].[Lead_Report_Sample] where EMAIL IN (SELECT Email FROM CRM.Leads where LeadStageId Is nULL);


SELECT DISTINCT Fortune_Ranking From [BSuit].[CRM].[Lead_Report_Sample];

-- SELECT Email FROM CRM.Leads where LeadStageId Is nULL;

SELECT Distinct LeadSourceId FROM CRM.Leads;

SELECT DISTINCT 
    LTRIM(RTRIM(value)) AS Service
FROM [BSuit].[CRM].[Lead_Report_Sample]
CROSS APPLY STRING_SPLIT(Main_Service, ',')
WHERE Main_Service IS NOT NULL


SELECT ld.LeadId, ld.Email, ld.EnquiryId, ls.Main_Service FROM CRM.Leads ld
Left Join CRM.Lead_Report_Sample ls ON ld.Email = ls.Email
WHERE ld.EnquiryId = ls.Customer_ID AND ls.Main_Service is NOT NULL;