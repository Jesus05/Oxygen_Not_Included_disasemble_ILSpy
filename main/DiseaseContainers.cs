using Database;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseContainers : KGameObjectComponentManager<DiseaseContainer>
{
	public HandleVector<int>.Handle Add(GameObject go, byte disease_idx, int disease_count)
	{
		DiseaseContainer diseaseContainer = new DiseaseContainer(go, disease_idx, disease_count);
		if (disease_idx != 255)
		{
			diseaseContainer = EvaluateGrowthConstants(diseaseContainer);
		}
		return Add(go, diseaseContainer);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		DiseaseContainer data = GetData(h);
		AutoDisinfectable autoDisinfectable = data.autoDisinfectable;
		if ((UnityEngine.Object)autoDisinfectable != (UnityEngine.Object)null)
		{
			AutoDisinfectableManager.Instance.RemoveAutoDisinfectable(autoDisinfectable);
		}
		base.OnCleanUp(h);
	}

	public override void Sim200ms(float dt)
	{
		for (int i = 0; i < data.Count; i++)
		{
			DiseaseContainer diseaseContainer = data[i];
			if (diseaseContainer.diseaseIdx != 255 && !((UnityEngine.Object)diseaseContainer.primaryElement == (UnityEngine.Object)null))
			{
				Disease disease = Db.Get().Diseases[diseaseContainer.diseaseIdx];
				float num = CalculateDelta(diseaseContainer, disease, dt);
				num += diseaseContainer.accumulatedError;
				int num2 = (int)num;
				diseaseContainer.accumulatedError = num - (float)num2;
				bool flag = diseaseContainer.diseaseCount > diseaseContainer.overpopulationCount;
				bool flag2 = diseaseContainer.diseaseCount + num2 > diseaseContainer.overpopulationCount;
				if (flag != flag2)
				{
					diseaseContainer = EvaluateGrowthConstants(diseaseContainer);
				}
				diseaseContainer.diseaseCount += num2;
				if (diseaseContainer.diseaseCount <= 0)
				{
					diseaseContainer.diseaseCount = 0;
					diseaseContainer.diseaseIdx = byte.MaxValue;
					diseaseContainer.accumulatedError = 0f;
				}
				data[i] = diseaseContainer;
			}
		}
	}

	public static float CalculateDelta(DiseaseContainer container, Disease disease, float dt)
	{
		int environment_cell = Grid.PosToCell(container.primaryElement.transform.GetPosition());
		return CalculateDelta(container.diseaseCount, container.elemIdx, container.primaryElement.Mass, environment_cell, container.primaryElement.Temperature, container.instanceGrowthRate, disease, dt);
	}

	public static float CalculateDelta(int disease_count, int element_idx, float mass, int environment_cell, float temperature, float tags_multiplier_base, Disease disease, float dt)
	{
		float num = 0f;
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[element_idx];
		num += elemGrowthInfo.CalculateDiseaseCountDelta(disease_count, mass, dt);
		float half_life_in_seconds = Disease.CalculateRangeHalfLife(temperature, ref disease.temperatureRange, ref disease.temperatureHalfLives);
		float num2 = Disease.HalfLifeToGrowthRate(half_life_in_seconds, dt);
		num += (float)disease_count * num2 - (float)disease_count;
		float num3 = Mathf.Pow(tags_multiplier_base, dt);
		num += (float)disease_count * num3 - (float)disease_count;
		if (Grid.IsValidCell(environment_cell))
		{
			byte b = Grid.ElementIdx[environment_cell];
			ElemExposureInfo elemExposureInfo = disease.elemExposureInfo[b];
			num += elemExposureInfo.CalculateExposureDiseaseCountDelta(disease_count, dt);
		}
		return num;
	}

	public int ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		DiseaseContainer data = GetData(h);
		data.diseaseCount = Math.Max(0, data.diseaseCount + disease_count_delta);
		if (data.diseaseCount == 0)
		{
			data.diseaseIdx = byte.MaxValue;
			data.accumulatedError = 0f;
		}
		SetData(h, data);
		return data.diseaseCount;
	}

	public int AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		DiseaseContainer diseaseContainer = GetData(h);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, diseaseContainer.diseaseIdx, diseaseContainer.diseaseCount);
		bool flag = diseaseContainer.diseaseIdx != diseaseInfo.idx;
		diseaseContainer.diseaseIdx = diseaseInfo.idx;
		diseaseContainer.diseaseCount = diseaseInfo.count;
		if (flag && diseaseInfo.idx != 255)
		{
			diseaseContainer = EvaluateGrowthConstants(diseaseContainer);
		}
		SetData(h, diseaseContainer);
		if (flag)
		{
			diseaseContainer.primaryElement.Trigger(-283306403, null);
		}
		return diseaseContainer.diseaseCount;
	}

	public void UpdateOverlayColours()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Database.Diseases diseases = Db.Get().Diseases;
		Color32 color = new Color32(0, 0, 0, byte.MaxValue);
		for (int i = 0; i < base.data.Count; i++)
		{
			DiseaseContainer diseaseContainer = base.data[i];
			KBatchedAnimController controller = diseaseContainer.controller;
			if ((UnityEngine.Object)controller != (UnityEngine.Object)null)
			{
				Color32 c = color;
				Vector3 position = controller.transform.GetPosition();
				if (visibleArea.Min <= (Vector2)position && (Vector2)position <= visibleArea.Max)
				{
					int num = 0;
					int disease_idx = 255;
					int disease_count = 0;
					diseaseContainer.GetVisualDiseaseIdxAndCount(out disease_idx, out disease_count);
					if (disease_idx != 255)
					{
						c = diseases[disease_idx].overlayColour;
						num = disease_count;
					}
					if (diseaseContainer.isContainer)
					{
						Storage component = diseaseContainer.primaryElement.GetComponent<Storage>();
						List<GameObject> items = component.items;
						for (int j = 0; j < items.Count; j++)
						{
							GameObject gameObject = items[j];
							if ((UnityEngine.Object)gameObject != (UnityEngine.Object)null)
							{
								HandleVector<int>.Handle handle = GetHandle(gameObject);
								if (handle.IsValid())
								{
									DiseaseContainer data = GetData(handle);
									if (data.diseaseCount > num && data.diseaseIdx != 255)
									{
										num = data.diseaseCount;
										c = diseases[data.diseaseIdx].overlayColour;
									}
								}
							}
						}
					}
					c.a = SimUtil.DiseaseCountToAlpha254(num);
					if (diseaseContainer.conduitType != 0)
					{
						ConduitFlow flowManager = Conduit.GetFlowManager(diseaseContainer.conduitType);
						int cell = Grid.PosToCell(position);
						ConduitFlow.ConduitContents contents = flowManager.GetContents(cell);
						if (contents.diseaseIdx != 255 && contents.diseaseCount > num)
						{
							num = contents.diseaseCount;
							c = diseases[contents.diseaseIdx].overlayColour;
							c.a = byte.MaxValue;
						}
					}
				}
				controller.OverlayColour = c;
			}
		}
	}

	private DiseaseContainer EvaluateGrowthConstants(DiseaseContainer container)
	{
		Disease disease = Db.Get().Diseases[container.diseaseIdx];
		KPrefabID component = container.primaryElement.GetComponent<KPrefabID>();
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[container.diseaseIdx];
		container.overpopulationCount = (int)(elemGrowthInfo.maxCountPerKG * container.primaryElement.Mass);
		container.instanceGrowthRate = disease.GetGrowthRateForTags(component.Tags, container.diseaseCount > container.overpopulationCount);
		return container;
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < data.Count; i++)
		{
			data[i].Clear();
		}
		data.Clear();
		handles.Clear();
	}
}
