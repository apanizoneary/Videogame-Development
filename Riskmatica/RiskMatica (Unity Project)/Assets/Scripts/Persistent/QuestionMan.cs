using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class QuestionMan : MonoBehaviour {

	private QuestionContainer qContainer;
	private string xmlPath;
	private int lastID;
	private int nextID;
	private int qCount;
	private List<Question> nextGameQuestions;

	public int QCount {
		get {
			return this.qCount;
		}
	}

	void Awake () {
		DontDestroyOnLoad(this);
		xmlPath = Application.dataPath+"/Questions.xml";
		if (System.IO.File.Exists (xmlPath)){
			qContainer = QuestionContainer.Load(xmlPath);
			qCount = qContainer.Questions.Count;
			nextID = qContainer.Questions [qCount - 1].qID+1;
		}
		else{
			qContainer = new QuestionContainer();
			qCount=0;
			nextID=1;
		}
		nextGameQuestions = new List<Question> (qContainer.Questions);
	}

	public int getNextID(){
		return nextID;
	}

	public List<Question> getQuestions(){
		return nextGameQuestions;
	}

	public void updateQManager(){
		qContainer = QuestionContainer.Load(xmlPath);
	}

	public void filterQuestions (string course,string theme,string difficulty){
		foreach(Question q in qContainer.Questions){
			if(theme!="" && q.theme != theme){
				nextGameQuestions.Remove(q);
			}
			if(difficulty!="" && q.difficulty != difficulty){
				nextGameQuestions.Remove(q);
			}
			if(course!="" && q.course != course){
				nextGameQuestions.Remove(q);
			}
		}
		if(nextGameQuestions.Count == 0){
			nextGameQuestions = qContainer.Questions;
		}
	}

	public List<string>[] getFilters(){
		List<string>[] filters = new List<string>[2];
		List<string> themes = new List<string> ();
		List<string> courses = new List<string> ();
		foreach (Question q in qContainer.Questions){
			if(!themes.Contains(q.theme)){
				themes.Add(q.theme);
			}
			if(!courses.Contains(q.course)){
				courses.Add(q.course);
			}
		}
		filters [0] = themes;
		filters [1] = courses;
		return filters;
	}

	public void resetNextGameQuestions(){
		nextGameQuestions = new List<Question> (qContainer.Questions);
	}

	public void addQuestion (Question question){
		qContainer.Questions.Add(question);
		qContainer.Save(xmlPath);
		qCount++;
		nextID++;
	}

	public void removeQuestion (Question question){
		qContainer.Questions.Remove(question);
		System.IO.File.Delete(Application.dataPath+"/Images/question_"+nextID+".png");
		qCount--;
	}

}
