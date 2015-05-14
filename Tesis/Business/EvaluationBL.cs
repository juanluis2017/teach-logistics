﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;
using Tesis.Models;
using Tesis.ViewModels;
using System.Web.Mvc;

namespace Tesis.Business
{
    public class EvaluationBL
    {
        public ICollection<EvaluationStudentViewModel> GetEvaluationStudent(ICollection<Evaluation> evaluations, string userId) {
            List<EvaluationStudentViewModel> evaluationStudentList = new List<EvaluationStudentViewModel>();
            foreach (var evaluation in evaluations)
            {
                EvaluationStudentViewModel evaluationStudentViewModel = new EvaluationStudentViewModel
                {
                    Id = evaluation.Id,
                    QuestionNumbers = evaluation.Questions.Count(),
                    QuizName = evaluation.Name,
                    TotalScore = evaluation.Questions.Sum(c => c.Score),
                    LimitDate = evaluation.LimitDate,
                    MinutesDuration = evaluation.MinutesDuration
                };
                EvaluationUser evaluationUser = evaluation.EvaluationUsers.Where(x => x.UserId == userId).FirstOrDefault();
                if (evaluationUser != null)
                {
                    evaluationStudentViewModel.GotScore = evaluationUser.Calification;
                    evaluationStudentViewModel.IsTaken = evaluationUser.Active;
                    evaluationStudentViewModel.TakenDate = evaluationUser.TakenDate;
                }
                evaluationStudentList.Add(evaluationStudentViewModel);
            }
            return evaluationStudentList;
        }

        public bool UserCanBeEvaluated(Evaluation evaluation, string UserId)
        {
            EvaluationUser evaluationUser = evaluation.EvaluationUsers.Where(x => x.UserId == UserId).FirstOrDefault();
            if (evaluationUser == null && evaluation.LimitDate >= DateTime.Today)
            {
                User user = evaluation.Sections.Where(x => x.Users.Select(y => y.Id).Contains(UserId)).SelectMany(z => z.Users.Where(c => c.Id == UserId)).FirstOrDefault();
                if (user != null)
                {
                    return true;
                }
            }
            return false;
        }

        public QuizViewModel GetQuiz(Evaluation evaluation, DateTime? evaluationTime = null)
        {
            List<QuestionQuizViewModel> questionViewModel = new List<QuestionQuizViewModel>();
            Random random = new Random();
            foreach (var question in evaluation.Questions)
            {
                Dictionary<Guid, string> dictionary = question.Options.OrderBy(x => random.Next()).ToDictionary(x => x.Id, x => x.Option);
                OptionQuizViewModel optionViewModel = new OptionQuizViewModel();
                optionViewModel.Options = new SelectList(dictionary, "Key", "Value");
                questionViewModel.Add(new QuestionQuizViewModel
                {
                    Id = question.Id,
                    ImagePath = question.ImagePath,
                    Options = optionViewModel,
                    QuestionText = question.QuestionText,
                    QuestionScore = question.Score
                });
            }
            return new QuizViewModel
            {
                Id = evaluation.Id,
                Questions = questionViewModel.OrderBy(x => random.Next()).ToList(),
                QuizName = evaluation.Name,
                Score = evaluation.Questions.Sum(x => x.Score),
                MinutesDuration = evaluation.MinutesDuration,
                StartTime = DateTime.Now,
            };
        }

        public void TakeQuiz(Evaluation evaluation, QuizViewModel quiz, string userId)
        {
            int TotalScore = 0;
            //Aquí validar si el quiz tiene las mismas preguntas que el quiz
            int questionNumber = quiz.Questions.Where(x => evaluation.Questions.Select(y => y.Id).Contains(x.Id)).Count();
            if (evaluation.Questions.Count() != questionNumber)
            {
                throw new Exception("El Quiz es falso");
            }
            //Validar si el estudiante ya presentó este quiz; tomar en cuenta el active
            EvaluationUser auxEvaluationUser = evaluation.EvaluationUsers.Where(x => x.UserId == userId).FirstOrDefault();
            if (auxEvaluationUser != null)
            {
                if (auxEvaluationUser.Active)
                {
                    throw new Exception("Estudiante ya presento este quiz");
                }
            }

            List<Answer> answers = new List<Answer>();
            foreach (var questionQuiz in quiz.Questions)
            {
                Question questionEvaluation = evaluation.Questions.Where(x => x.Id == questionQuiz.Id).FirstOrDefault();
                Guid correctOption = questionEvaluation.Options.Where(x => x.IsCorrectOption == true).Select(y => y.Id).FirstOrDefault();
                if (correctOption == questionQuiz.Options.SelectedAnswer)
                {
                    TotalScore += questionEvaluation.Score;
                }
                answers.Add(new Answer
                {
                    QuestionOptionId = questionQuiz.Options.SelectedAnswer
                });
            }

            EvaluationUser evaluationUser = new EvaluationUser
            {
                TakenDate = DateTime.Now,
                UserId = userId,
                EvaluationId = evaluation.Id,
                Calification = TotalScore,
                Answers = answers,
                Active = true,
            };
            evaluation.EvaluationUsers.Add(evaluationUser);
        }

        public QuizViewModel ReviewQuiz(Evaluation evaluation, string userId, bool isUserRequired = false)
        {
            EvaluationUser evaluationUser = evaluation.EvaluationUsers.Where(x => x.UserId == userId).FirstOrDefault();
            List<QuestionQuizViewModel> quizQuestions = new List<QuestionQuizViewModel>();
            foreach (var question in evaluation.Questions)
            {
                Dictionary<Guid, string> dictionary = question.Options.ToDictionary(x => x.Id, x => x.Option);
                OptionQuizViewModel optionViewModel = new OptionQuizViewModel
                {
                    CorrectAnswer = question.Options.Where(x => x.IsCorrectOption == true).Select(y => y.Id).FirstOrDefault(),
                    SelectedAnswer = evaluationUser.Answers.Where(x => question.Options.Select(y => y.Id).Contains(x.QuestionOptionId)).Select(z => z.QuestionOptionId).FirstOrDefault(),
                    Options = new SelectList(dictionary, "Key", "Value")
                };

                QuestionQuizViewModel quizQuestion = new QuestionQuizViewModel
                {
                    ImagePath = question.ImagePath,
                    QuestionScore = question.Score,
                    QuestionText = question.QuestionText,
                    Options = optionViewModel
                };
                quizQuestions.Add(quizQuestion);
            }
            QuizViewModel quizViewModel = new QuizViewModel
            {
                Id = evaluation.Id,
                QuizName = evaluation.Name,
                Score = evaluation.Questions.Sum(x => x.Score),
                GotScore = evaluationUser.Calification,
                Questions = quizQuestions,
                User = (isUserRequired) ? evaluationUser.User : null,
            };
            return quizViewModel;
        }
    }
}