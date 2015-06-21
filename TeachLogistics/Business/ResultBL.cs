﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeachLogistics.Models;
using TeachLogistics.ViewModels;

namespace TeachLogistics.Business
{
    public class ResultBL
    {
        public ICollection<GroupResultViewModel> GetGroupsResult(Section section)
        {
            List<Group> groups = section.Groups.Where(x => x.IsInSimulation == true).ToList();
            List<GroupResultViewModel> groupsResult = new List<GroupResultViewModel>();
            foreach (Group group in groups)
            {
                GroupResultViewModel groupResult = new GroupResultViewModel {
                    GroupId = group.Id,
                    GroupName = group.Name,
                    //GroupDetailedResult = GetGroupResults(group),
                };
                groupsResult.Add(groupResult);
            }
            return groupsResult;
        }

        public List<GroupDetailedResultViewModel> GetGroupResults(Group group)
        {
            var results = group.Balances
                .OrderBy(x => x.Period.Created)
                .GroupBy(x => x.Period)
                .Select(t  => new GroupDetailedResultViewModel
                {
                    PeriodNumber = 1,
                    FinalStockCost = t.Key.Balances.Sum(x => x.FinalStockCost),
                    UnsatisfiedDemandCost = t.Key.Balances.Sum(x => x.DissatisfiedCost),
                    TotalOrderCost = t.Key.Balances.Sum(x => x.FinalStockCost) + t.Key.Balances.Sum(x => x.DissatisfiedCost)
                })
                .ToList();
            return results;
        }
    }
}