/** @jsxImportSource @emotion/react */
import { css } from '@emotion/react';
import React from 'react';
import { useSearchParams } from 'react-router-dom';
import { QuestionList } from './QuestionList';
import { QuestionData, searchQuestions } from './QuestionsData';
import { Page } from './Page';

export const SearchPage = () => {
  const [searchParams] = useSearchParams();
  const [questions, setQuestions] = React.useState<QuestionData[]>([]);

  const search = searchParams.get('criteria') || '';

  React.useEffect(() => {
    let cancelled = false;
    const doSearch = async (criteria: string) => {
      const foundResults = await searchQuestions(criteria);
      if (!cancelled) {
        setQuestions(foundResults);
      }
    };
    doSearch(search);
    return () => {
      cancelled = true;
    };
  }, [search]);
  return (
    <Page title="Search Results">
      {
        <p
          css={css`
            font-size: 16px;
            font-style: italic;
            margin-top: 0px;
          `}
        >
          {search ? `for "${search}"` : ''}
        </p>
      }
      <QuestionList data={questions} />
    </Page>
  );
};
