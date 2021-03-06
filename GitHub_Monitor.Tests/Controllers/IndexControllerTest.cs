﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GitHub_Monitor.Controllers;
using GitHub_Monitor.Models;
using GitHub_Monitor.Services;
using GitHub_Monitor.Tests.TestUtilities;
using Moq;
using NUnit.Framework;

namespace GitHub_Monitor.Tests.Controllers
{
	[TestFixture]
	public class IndexControllerTest
	{
		#region Dependencies
		private IndexController indexController;
		private Mock<IRepositoryService> mockRepositoryService;

		[SetUp]
		public void SetUp()
		{
			mockRepositoryService = new Mock<IRepositoryService>();

			indexController = new IndexController() { RepositoryService = mockRepositoryService.Object };
		}
		#endregion

		#region Index
		[Test]
		public async Task IndexController_ShouldReturnRepositories_WhenThereAreRepositories()
		{
			// Setup
			var expectedRepositories = Enumerable.Range(1, 7).Select(Generator.GenerateRepository).ToList();
			mockRepositoryService.Setup(s => s.GetAll()).Returns(() => Task.FromResult(expectedRepositories as IEnumerable<Repository>));

			// Execute
			var result = await indexController.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsAssignableFrom<List<Repository>>(result.ViewData.Model);
			var actualRepositories = result.ViewData.Model as List<Repository>;

			AssertUtils.AreIdentical(expectedRepositories, actualRepositories);
		}


		[Test]
		public async Task IndexController_ShouldReturnEmptySet_WhenThereAreNoRepositories()
		{
			// Setup
			IEnumerable<Repository> expectedRepositories = new List<Repository>();
			mockRepositoryService.Setup(s => s.GetAll()).Returns(() => Task.FromResult(expectedRepositories));

			// Execute
			var result = await indexController.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.IsAssignableFrom<List<Repository>>(result.ViewData.Model);
			var actualRepositories = result.ViewData.Model as List<Repository>;

			Assert.IsNotNull(actualRepositories);
			Assert.AreEqual(0, actualRepositories.Count);
		}


		[Test]
		public async Task IndexController_ShouldProvideErrorMessage_WhenRepositoryServiceThrowsError()
		{
			// Setup
			mockRepositoryService.Setup(s => s.GetAll()).Throws(new ArgumentException("Invalid Configuration"));

			// Execute
			var result = await indexController.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("Server Error, please try again later", result.ViewBag.Message);
		}
		#endregion
	}
}
