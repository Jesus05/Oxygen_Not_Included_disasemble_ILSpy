using UnityEngine;

namespace Klei.AI
{
	public class AnimatedDisease : Disease.DiseaseComponent
	{
		private KAnimFile[] kanims = null;

		private Expression expression;

		public AnimatedDisease(HashedString[] kanim_filenames, Expression expression)
		{
			kanims = new KAnimFile[kanim_filenames.Length];
			for (int i = 0; i < kanim_filenames.Length; i++)
			{
				kanims[i] = Assets.GetAnim(kanim_filenames[i]);
			}
			this.expression = expression;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			for (int i = 0; i < kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().AddAnimOverrides(kanims[i], 10f);
			}
			if (expression != null)
			{
				go.GetComponent<FaceGraph>().AddExpression(expression);
			}
			return null;
		}

		public override void OnCure(GameObject go, object instace_data)
		{
			if (expression != null)
			{
				go.GetComponent<FaceGraph>().RemoveExpression(expression);
			}
			for (int i = 0; i < kanims.Length; i++)
			{
				go.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(kanims[i]);
			}
		}
	}
}
