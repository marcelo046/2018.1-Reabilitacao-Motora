using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.EventSystems;
using Mono.Data.Sqlite;

using pessoa;
using fisioterapeuta;
using paciente;
using musculo;
using movimento;
using sessao;
using exercicio;
using movimentomusculo;
using pontosrotulofisioterapeuta;
using pontosrotulopaciente;

namespace Tests
{
	public static class TestDelete
	{
		[SetUp]
		public static void BeforeEveryTest ()
		{
			GlobalController.test = true;
			GlobalController.Initialize();
		}

		[UnityTest]
		public static IEnumerator TestDeleteMovement()
		{
			Flow.StaticLogin();

			yield return new WaitForSeconds(1f);

			Pessoa.Insert("physio name1", "m", "1995-01-01", "6198732711", null);
			Fisioterapeuta.Insert(1, "abracadabra1", "demais1", null, null);

			var fisio = Fisioterapeuta.Read();

			GlobalController.instance.admin = fisio[fisio.Count - 1];

			Flow.StaticNewMovement();

			yield return new WaitForSeconds(1f);
			var moveManager = GameObject.Find("/New Movement Manager").GetComponentInChildren<createMovement>();

			InputField aux = (InputField)moveManager.GetMemberValue("nomeMovimento");
			aux.text = "Fake Name";
			moveManager.SetMemberValue("nomeMovimento", aux);

			InputField aux1 = (InputField)moveManager.GetMemberValue("musculos");
			aux1.text = "Deltoide";
			moveManager.SetMemberValue("musculos", aux1);
			
			InputField aux3 = (InputField)moveManager.GetMemberValue("descricao");
			aux3.text = "Ahaha seloco";
			moveManager.SetMemberValue("descricao", aux3);
			
			moveManager.saveMovement();

			yield return new WaitForSeconds(1f);

			DeleteMovementButton.DeleteMovement();

			yield return new WaitForSeconds(0.5f);

			var currentscene = SceneManager.GetActiveScene().name;
			var expectedscene = "Movements";

			Assert.AreEqual(expectedscene, currentscene);

			int IdMovimento = GlobalController.instance.movement.idMovimento;

			var prfs = PontosRotuloFisioterapeuta.Read();
			foreach (var prf in prfs)
			{
				Assert.AreNotEqual(prf.idMovimento, IdMovimento);
			}

			var exers = Exercicio.Read();
			foreach (var exer in exers)
			{
				Assert.AreNotEqual(exer.idMovimento, IdMovimento);
			}

			var mms = MovimentoMusculo.Read();
			foreach (var mm in mms)
			{
				Assert.AreNotEqual(mm.idMovimento, IdMovimento);
			}

			string pathMov = string.Format("{0}/Movimentos/{1}", Application.dataPath, GlobalController.instance.movement.pontosMovimento);

			bool dir = System.IO.File.Exists(pathMov);

			Assert.AreEqual (dir, false);
		}

		[UnityTest]
		public static IEnumerator TestDeletePatient()
		{
			Flow.StaticLogin();
			Flow.StaticNewPatient();

			yield return null;
			
			var objectPatient = GameObject.Find("PatientManager");
			var PatientManager = objectPatient.GetComponentInChildren<createPatient>();

			var objectButton = GameObject.Find("Canvas/PanelPatient/SaveBt");
			var button = objectButton.GetComponentInChildren<Button>();

			InputField aux = (InputField)PatientManager.GetMemberValue("namePatient");
			aux.text = "Fake Name";
			PatientManager.SetMemberValue("namePatient", aux);

			InputField aux1 = (InputField)PatientManager.GetMemberValue("date");
			aux1.text = "01/01/1920";
			PatientManager.SetMemberValue("date", aux1);
			
			InputField aux3 = (InputField)PatientManager.GetMemberValue("phone1");
			aux3.text = "61999999";
			PatientManager.SetMemberValue("phone1", aux3);

			InputField aux8 = (InputField)PatientManager.GetMemberValue("notes");
			aux8.text = "lorem ipsum";
			PatientManager.SetMemberValue("notes", aux8);

			Toggle aux2 = (Toggle)PatientManager.GetMemberValue("male");
			aux2.isOn = true;
			PatientManager.SetMemberValue("male", aux2);

			Toggle aux0 = (Toggle)PatientManager.GetMemberValue("female");
			aux0.isOn = false;
			PatientManager.SetMemberValue("female", aux0);

			PatientManager.savePatient();

			int IdPaciente = GlobalController.instance.user.idPaciente;
			int IdPessoa = GlobalController.instance.user.persona.idPessoa;

			yield return new WaitForSeconds(0.5f);

			DeletePatientButton.DeletePatient();

			yield return new WaitForSeconds(0.5f);

			var currentscene = SceneManager.GetActiveScene().name;
			var expectedscene = "NewPatient";

			Assert.AreEqual(expectedscene, currentscene);

			var patients = Paciente.Read();
			foreach (var patient in patients)
			{
				Assert.AreNotEqual(patient.idPaciente, IdPaciente);
			}

			var exers = Exercicio.Read();
			foreach (var exer in exers)
			{
				Assert.AreNotEqual(exer.idPaciente, IdPaciente);
			}

			var sess = Sessao.Read();
			foreach (var ses in sess)
			{
				Assert.AreNotEqual(ses.idPaciente, IdPaciente);
			}

			var ppls = Pessoa.Read();
			foreach (var ppl in ppls)
			{
				Assert.AreNotEqual(ppl.idPessoa, IdPessoa);
			}

			string nomePessoa = (GlobalController.instance.user.persona.nomePessoa).Replace(' ', '_');
			string nomePasta = string.Format("Assets\\Exercicios\\{1}-{2}", Application.dataPath, IdPessoa, nomePessoa);

			bool dir = System.IO.File.Exists(nomePasta.Replace('/', '\\'));

			Assert.AreEqual (dir, false);
		}


		[TearDown]
		public static void AfterEveryTest ()
		{
			SqliteConnection.ClearAllPools();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GlobalController.DropAll();
			GlobalController.instance = null;
		}
	}
}
