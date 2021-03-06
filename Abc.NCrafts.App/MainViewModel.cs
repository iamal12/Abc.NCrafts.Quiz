﻿using System;
using System.Configuration;
using System.IO;
using Abc.NCrafts.App.ViewModels;
using Abc.NCrafts.App.ViewModels.Questions;
using PropertyChanged;

namespace Abc.NCrafts.App
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        private readonly WelcomePage _welcomePage;
        private readonly AllocationGamePage _allocationGamePage;
        private readonly PerformanceGamePage _performanceGamePage;
        private readonly EndPage _endPage;

        public MainViewModel()
        {
            EnsureQuizCanBeLoaded();

            _welcomePage = new WelcomePage(this);
            _allocationGamePage = new AllocationGamePage(this);
            _performanceGamePage = new PerformanceGamePage(this);
            _endPage = new EndPage(this);

            _allocationGamePage.NextPage = _endPage;
            _performanceGamePage.NextPage = _endPage;
            _endPage.NextPage = _welcomePage;

            CurrentPage = _welcomePage;
        }

        public Quiz Quiz { get; private set; }
        public ViewModel CurrentPage { get; set; }

        public void StartGame(QuizzType quizzType)
        {
            Quiz = LoadQuiz(quizzType);

            switch (quizzType)
            {
                case QuizzType.Performance:
                    _welcomePage.NextPage = _performanceGamePage;
                    _endPage.PreviousPage = _performanceGamePage;
                    break;
                case QuizzType.Allocation:
                    _welcomePage.NextPage = _allocationGamePage;
                    _endPage.PreviousPage = _allocationGamePage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(quizzType), quizzType, null);
            }
        }

        private static void EnsureQuizCanBeLoaded()
        {
            LoadQuiz(QuizzType.Allocation);
            LoadQuiz(QuizzType.Performance);
        }

        private static Quiz LoadQuiz(QuizzType quizzType)
        {
            return QuizLoader.LoadFrom(GetQuizPath(quizzType));
        }

        private static string GetQuizPath(QuizzType quizzType)
        {
            var quizPathRoot = GetQuizPathRoot();
            if (string.IsNullOrEmpty(quizPathRoot))
                throw new InvalidOperationException("Unable to locate quiz path, you should set \"Quiz.Path\" setting key.");

            return Path.Combine(quizPathRoot, quizzType.ToString(), "Questions");
        }

        private static string GetQuizPathRoot()
        {
            var quizPath = ConfigurationManager.AppSettings["Quiz.Path"];
            return string.IsNullOrEmpty(quizPath) ? FindQuizPath() : quizPath;
        }

        private static string FindQuizPath()
        {
            var baseDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            while (baseDirectory != null)
            {
                var parentDirectory = Path.GetDirectoryName(baseDirectory);
                if (Path.GetFileName(baseDirectory) == "Abc.NCrafts.App" && parentDirectory != null)
                    return Path.Combine(parentDirectory, "Abc.NCrafts.Quizz");

                baseDirectory = parentDirectory;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}