using DrugPrioritizationAssistant.Application.DTOs;
using DrugPrioritizationAssistant.Domain.Entities;

namespace DrugPrioritizationAssistant.Application.Services;

public class DrugPriorityExplanationService
    : IDrugPriorityExplanationService
{
    public DrugPriorityExplanationDto Generate(
        Drug drug,
        DrugNeedAssessment assessment,
        ProductionFeasibility feasibility,
        DrugScore score)
    {
        var reasons =
            new List<DrugPriorityReasonDto>();

        AddNeedReasons(
            reasons,
            assessment);

        AddFeasibilityReasons(
            reasons,
            feasibility);

        var topReasons = reasons
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        return new DrugPriorityExplanationDto
        {
            DrugId = drug.Id,

            DrugName = drug.Name,

            NeedScore =
        assessment.NeedScore,

            FeasibilityScore =
        feasibility.FeasibilityScore,

            OpportunityScore =
        score.OpportunityScore,

            Rank =
        score.Rank,

            PriorityLevel =
        GetPriorityLevel(
            score.OpportunityScore),

            Reasons = topReasons
        };
    }

    private static void AddNeedReasons(
        List<DrugPriorityReasonDto> reasons,
        DrugNeedAssessment assessment)
    {
        if (assessment.ShortageSeverity >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "شدت کمبود بالا",

                    Score =
                        assessment.ShortageSeverity,

                    Description =
                        "شدت کمبود این دارو در سطح بالایی ارزیابی شده است.",

                    Type =
                        "need"
                });
        }

        if (assessment.ImportDependency >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "وابستگی وارداتی بالا",

                    Score =
                        assessment.ImportDependency,

                    Description =
                        "وابستگی بالای دارو به واردات، ریسک تأمین داخلی را افزایش می‌دهد.",

                    Type =
                        "need"
                });
        }

        if (assessment.DemandLevel >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "تقاضای بالا",

                    Score =
                        assessment.DemandLevel,

                    Description =
                        "سطح تقاضای دارو بالا ارزیابی شده است.",

                    Type =
                        "need"
                });
        }

        if (assessment.TherapeuticImportance >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "اهمیت درمانی بالا",

                    Score =
                        assessment.TherapeuticImportance,

                    Description =
                        "اهمیت درمانی بالای دارو، نیاز به تأمین پایدار آن را افزایش می‌دهد.",

                    Type =
                        "need"
                });
        }

        if (assessment.SupplyInstability >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "ناپایداری تأمین",

                    Score =
                        assessment.SupplyInstability,

                    Description =
                        "ریسک ناپایداری تأمین این دارو بالا ارزیابی شده است.",

                    Type =
                        "need"
                });
        }
    }

    private static void AddFeasibilityReasons(
        List<DrugPriorityReasonDto> reasons,
        ProductionFeasibility feasibility)
    {
        if (feasibility.RawMaterialAvailability >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "دسترسی مناسب به مواد اولیه",

                    Score =
                        feasibility.RawMaterialAvailability,

                    Description =
                        "دسترسی به مواد اولیه در سطح مناسبی ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }

        if (feasibility.ExpectedYield >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "بازده مورد انتظار مناسب",

                    Score =
                        feasibility.ExpectedYield,

                    Description =
                        "بازده مورد انتظار فرآیند تولید در سطح مناسبی ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }

        if (feasibility.PurityPotential >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "پتانسیل خلوص مناسب",

                    Score =
                        feasibility.PurityPotential,

                    Description =
                        "پتانسیل دستیابی به خلوص مناسب ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }

        if (feasibility.ScaleUpFeasibility >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "امکان Scale-up مناسب",

                    Score =
                        feasibility.ScaleUpFeasibility,

                    Description =
                        "امکان توسعه فرآیند از مقیاس آزمایشگاهی به مقیاس بالاتر مناسب ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }

        if (feasibility.EquipmentAvailability >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "دسترسی مناسب به تجهیزات",

                    Score =
                        feasibility.EquipmentAvailability,

                    Description =
                        "دسترسی به تجهیزات مورد نیاز برای تولید مناسب ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }

        if (feasibility.EnzymaticRoutePotential >= 80)
        {
            reasons.Add(
                new DrugPriorityReasonDto
                {
                    Title =
                        "پتانسیل مسیر آنزیمی",

                    Score =
                        feasibility.EnzymaticRoutePotential,

                    Description =
                        "پتانسیل استفاده از مسیر آنزیمی برای تولید این دارو بالا ارزیابی شده است.",

                    Type =
                        "feasibility"
                });
        }
    }

    private static string GetPriorityLevel(
        decimal opportunityScore)
    {
        if (opportunityScore >= 80)
        {
            return "خیلی بالا";
        }

        if (opportunityScore >= 65)
        {
            return "بالا";
        }

        if (opportunityScore >= 50)
        {
            return "متوسط";
        }

        return "پایین";
    }
}