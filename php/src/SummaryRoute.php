<?php

declare(strict_types=1);

namespace MoodTracker\Php;

use PDO;
use PDOException;
use Psr\Http\Message\ResponseInterface;
use Psr\Http\Message\ServerRequestInterface;
use Throwable;

final class SummaryRoute
{
    private const TABLE = 'MoodEntries';

    public function __construct(private readonly ?PDO $pdo)
    {
    }

    public function handle(ServerRequestInterface $_request, ResponseInterface $response): ResponseInterface
    {
        $entries = [];
        $counts = [];

        if ($this->pdo !== null) {
            try {
                $entries = $this->loadEntries();
                $counts = $this->countByMood();
            } catch (Throwable) {
                $entries = [];
                $counts = [];
            }
        }

        $html = $this->render($entries, $counts);
        $response->getBody()->write($html);

        return $response->withHeader('Content-Type', 'text/html; charset=utf-8');
    }

    private function loadEntries(): array
    {
        try {
            $stmt = $this->pdo->query(
                'SELECT Id, Mood, Note, LoggedAt FROM ' . self::TABLE . ' ORDER BY LoggedAt DESC LIMIT 100'
            );
            return $stmt === false ? [] : $stmt->fetchAll();
        } catch (PDOException) {
            return [];
        }
    }

    private function countByMood(): array
    {
        try {
            $stmt = $this->pdo->query(
                'SELECT Mood, COUNT(*) AS Total FROM ' . self::TABLE . ' GROUP BY Mood ORDER BY Total DESC'
            );
            if ($stmt === false) {
                return [];
            }
            $rows = $stmt->fetchAll();
            $result = [];
            foreach ($rows as $row) {
                $result[$row['Mood']] = (int) $row['Total'];
            }
            return $result;
        } catch (PDOException) {
            return [];
        }
    }

    private function render(array $entries, array $counts): string
    {
        ob_start();
        include __DIR__ . '/../views/summary.phtml';
        return ob_get_clean() ?: '';
    }
}
