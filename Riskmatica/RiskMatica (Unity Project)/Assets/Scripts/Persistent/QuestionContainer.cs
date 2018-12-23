using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("QuestionContainer")]
public class QuestionContainer{
	[XmlArray("Questions"),XmlArrayItem("Question")]
	public List<Question> Questions = new List<Question>();

	public QuestionContainer ()
	{
	}

	public void Save(string xmlPath)
	{
		var serializer = new XmlSerializer(typeof(QuestionContainer));
		using(var stream = new FileStream(xmlPath, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static QuestionContainer Load(string xmlPath)
	{
		var serializer = new XmlSerializer(typeof(QuestionContainer));
		using(var stream = new FileStream(xmlPath, FileMode.Open))
		{
			return serializer.Deserialize(stream) as QuestionContainer;
		}
	}

}